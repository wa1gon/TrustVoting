namespace VoterRegistrarServer.Service;

using System;
using System.Text;
using NATS.Client;
using NATS.Client.JetStream;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;

public class VoterRegistrarService
{
    private readonly IConnection _connection;
    private readonly IJetStream _jetStream;
    private readonly string _voterRegistrarSubject = "election.voter.register";
    private readonly string _ackSubject = "election.acknowledge";

    public VoterRegistrarService(IConnection connection)
    {
        _connection = connection;
        _jetStream = _connection.CreateJetStreamContext();
    }

    // Method to send a message to ElectionAuthorityService.Core
    public async Task SendSignedPayloadAsync(VoterRegistration voterRegistration, RSA privateKey)
    {
        // Serialize the voter registration object to JSON
        string jsonPayload = JsonConvert.SerializeObject(voterRegistration);

        // Sign the payload
        string signedPayload = SignPayload(jsonPayload, privateKey);

        // Construct the message
        var messageContent = new
        {
            Payload = jsonPayload,
            Signature = signedPayload
        };

        string messageJson = JsonConvert.SerializeObject(messageContent);
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);

        // Send message to ElectionAuthorityService.Core
        PublishAck ack = await _jetStream.PublishAsync(_voterRegistrarSubject, messageBytes);
        Console.WriteLine($"Message sent to ElectionAuthorityService.Core. ACK: {ack}");
    }

    // Method to receive acknowledgment or response from ElectionAuthorityService.Core
    public void StartReceivingAcknowledgments()
    {
        // // Create a subscription to listen for acknowledgments
        // IJetStreamPullSubscription subscription = _jetStream.PullSubscribe(_ackSubject, "ack-durable");
        //
        // Task.Run(async () =>
        // {
        //     while (true)
        //     {
        //         // Fetch acknowledgment messages
        //         var messages = subscription.Fetch(10, 1000); // Fetch 10 messages at a time, waiting for up to 1 second
        //
        //         foreach (var msg in messages)
        //         {
        //             string messageData = Encoding.UTF8.GetString(msg.Data);
        //             Console.WriteLine($"Received acknowledgment: {messageData}");
        //             msg.Ack(); // Acknowledge the message
        //         }
        //
        //         await Task.Delay(1000); // Wait 1 second before checking again
        //     }
        // });
    }

    // Method to sign the payload using RSA
    private string SignPayload(string jsonPayload, RSA privateKey)
    {
        byte[] payloadBytes = Encoding.UTF8.GetBytes(jsonPayload);
        byte[] signedBytes = privateKey.SignData(payloadBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signedBytes);
    }
}

// Define a simple VoterRegistration class
public class VoterRegistration
{
    public string VoterId { get; set; }
    public string FullName { get; set; }
    public DateTime RegistrationDate { get; set; }
    // Additional properties...
}
