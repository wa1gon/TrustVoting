using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using ElectionAuthorityService.Services;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;


namespace ElectionAuthorityService.Pages;
public class StreamUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string[] Subjects { get; set; } = Array.Empty<string>();
    public StreamConfigStorage Storage { get; set; }
    public long MaxBytes { get; set; }
    public long MaxMsgs { get; set; }
    public long MaxAge { get; set; }
}
public partial class JetStreamPage : ComponentBase
{
    // [Inject]
    // JetSteamService JetStreamService { get; set; }
    [Inject] ILogger<JetStreamPage> logger { get; set; }
    private string message;
    private List<string> messages = new List<string>();

    protected override async Task OnInitializedAsync()
    {
       using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger("NATS-by-Example");


var url = Environment.GetEnvironmentVariable("NATS_URL") ?? "127.0.0.1:4222";


var opts = new NatsOpts
{
    Url = url,
    LoggerFactory = loggerFactory,
    Name = "NATS-by-Example",
};
await using var nats = new NatsConnection(opts);


var js = new NatsJSContext(nats);


var config = new StreamConfig(name: "EVENTS", subjects: new [] { "events.>" });


config.Storage = StreamConfigStorage.File;


var stream = await js.CreateStreamAsync(config);


for (var i = 0; i < 2; i++)
{
    await js.PublishAsync<object>(subject: "events.page_loaded", data: null);
    await js.PublishAsync<object>(subject: "events.mouse_clicked", data: null);
    await js.PublishAsync<object>(subject: "events.mouse_clicked", data: null);
    await js.PublishAsync<object>(subject: "events.page_loaded", data: null);
    await js.PublishAsync<object>(subject: "events.mouse_clicked", data: null);
    await js.PublishAsync<object>(subject: "events.input_focused", data: null);
    logger.LogInformation("Published 6 messages");
}


await PrintStreamStateAsync(stream);


// var configUpdate = new StreamUpdateRequest { Name = config.Name, Subjects = config.Subjects, Storage = config.Storage };


// configUpdate.MaxMsgs = 10;
// await js.UpdateStreamAsync(configUpdate);
logger.LogInformation("set max messages to 10");


await PrintStreamStateAsync(stream);


// configUpdate.MaxBytes = 300;
// await js.UpdateStreamAsync(configUpdate);
logger.LogInformation("set max bytes to 300");


await PrintStreamStateAsync(stream);


// configUpdate.MaxAge = (long)TimeSpan.FromSeconds(1).TotalNanoseconds;
// await js.UpdateStreamAsync(configUpdate);
logger.LogInformation("set max age to one second");


await PrintStreamStateAsync(stream);


logger.LogInformation("sleeping one second...");
await Task.Delay(TimeSpan.FromSeconds(1));


await PrintStreamStateAsync(stream);
    
    

logger.LogInformation("Bye!");


async Task PrintStreamStateAsync(INatsJSStream jsStream)
{
    await jsStream.RefreshAsync();
    var state = jsStream.Info.State;
    logger.LogInformation("Stream has {Messages} messages using {Bytes} bytes", state.Messages, state.Bytes);
}
    }
    async Task PrintStreamStateAsync(INatsJSStream jsStream)
    {
        await jsStream.RefreshAsync();
        var state = jsStream.Info.State;
        logger.LogInformation("Stream has {Messages} messages using {Bytes} bytes", state.Messages, state.Bytes);
    }
    private async Task PublishMessage()
    {
        // await JetStreamService.PublishAsync("foo", message);
    }
}
