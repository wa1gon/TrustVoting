namespace ElectionAuthorityService.Services;

public static class AppExtension
{
    public static void SerilogConfiguration(this IHostBuilder host)
    {
        host.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig.WriteTo.Console();
            loggerConfig.WriteTo.File("c:/Voteritoas/logs/log.txt", rollingInterval: RollingInterval.Day);
        });
    }
}
