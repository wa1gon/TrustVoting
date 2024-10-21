using FluentResults;

namespace TrustedVoteLibrary.Utils;

public  static class JetStreamUtils
{
    public static Result EnsureStreamExists(IConnection connection, string streamName, string subject, ILogger logger)
    {
        try
        {
            Log.Information("Top of EnsureStreamExists");
            IJetStreamManagement jsm = connection.CreateJetStreamManagementContext();
            StreamInfo streamInfo = jsm.GetStreamInfo(streamName);
            // logger.LogInformation().LogInformation($"Stream '{streamName}' already exists.");
            return Result.Ok();
        }
        catch (NATSJetStreamException ex)
        {
            if (ex.ErrorCode == 404) // Stream not found
            {
                logger.Information($"Stream '{streamName}' does not exist. Creating it now...");

                StreamConfiguration streamConfig = StreamConfiguration.Builder()
                    .WithName(streamName)
                    .WithSubjects(subject)
                    .Build();

                IJetStreamManagement jsm = connection.CreateJetStreamManagementContext();
                jsm.AddStream(streamConfig);

                logger.Information($"Stream '{streamName}' created successfully.");
                return Result.Ok();
            }
            else
            {
                logger.Error(ex, $"Error checking or creating stream '{streamName}'");
                return Result.Fail(ex.Message);
            }
        }
    }
}
