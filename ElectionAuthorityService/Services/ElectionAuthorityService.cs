using TrustedVoteLibrary.Utils;
namespace ElectionAuthorityService.Services;

public class ElectionAuthorityService(Serilog.ILogger logger) : IHostedService, IDisposable
{
    private IConnection connection;
    private IJetStreamPullSubscription subscription;
    private Timer timer;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Information("ElectionAuthorityService is starting.");

        PullSubscribeOptions pullOptions = PullSubscribeOptions.Builder()
            .WithDurable("durable-election-authority")
            .Build();
        // Initialize NATS connection and JetStream subscription
        var options = ConnectionFactory.GetDefaultOptions();
        options.Url = "nats://localhost:4222"; // NATS server URL

        connection = new ConnectionFactory().CreateConnection(options);
        logger.Information("before checking for stream");
        JetStreamUtils.EnsureStreamExists(connection, "VoterRegistrationStream", "election.voter.register", logger);
        IJetStream js = connection.CreateJetStreamContext();

        // Subscribe to the JetStream subject with a durable subscription
        subscription = js.PullSubscribe("election.voter.register", pullOptions);

        // Start processing messages periodically
        timer = new Timer(ProcessMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    private void ProcessMessages(object state)
    {
        logger.Information("Checking for messages...");

        try
        {
            var messages = subscription.Fetch(10, 1000); // Fetch 10 messages or wait for max 1000 ms

            foreach (var msg in messages)
            {
                string messageContent = Encoding.UTF8.GetString(msg.Data);
                dynamic receivedData = JsonConvert.DeserializeObject(messageContent);

                string jsonPayload = receivedData.Payload;
                string signature = receivedData.Signature;

                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportFromPem(File.ReadAllText("public_key.pem"));

                    if (VerifySignature(jsonPayload, signature, rsa))
                    {
                        logger.Information("Signature verified! Valid payload.");
                        logger.Information($"Payload: {jsonPayload}");
                    }
                    else
                    {
                        logger.Warning("Signature verification failed.");
                    }
                }

                // Acknowledge the message after processing
                msg.Ack();
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error while processing messages.");
        }
    }

    private bool VerifySignature(string jsonPayload, string signature, RSA publicKey)
    {
        byte[] payloadBytes = Encoding.UTF8.GetBytes(jsonPayload);
        byte[] signatureBytes = Convert.FromBase64String(signature);

        return publicKey.VerifyData(payloadBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.Information("ElectionAuthorityService is stopping.");
        timer?.Change(Timeout.Infinite, 0);
        connection?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose();
        connection?.Dispose();
    }
}
