using ElectionAuthorityService.Areas.Identity;
using ElectionAuthorityService.Data;
using ElectionAuthorityService.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var darkTheme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
{
    #region program.cs serilog logger

    [ConsoleThemeStyle.Text] = "\x1b[37m", // White text
    [ConsoleThemeStyle.SecondaryText] = "\x1b[30m", // Black
    [ConsoleThemeStyle.TertiaryText] = "\x1b[30m", // Black
    [ConsoleThemeStyle.Invalid] = "\x1b[31m", // Red
    [ConsoleThemeStyle.Null] = "\x1b[1;30m", // Gray
    [ConsoleThemeStyle.Name] = "\x1b[1;37m", // Bold white
    [ConsoleThemeStyle.String] = "\x1b[1;33m", // Yellow
    [ConsoleThemeStyle.Number] = "\x1b[1;32m", // Green
    [ConsoleThemeStyle.Boolean] = "\x1b[1;32m", // Green
    [ConsoleThemeStyle.Scalar] = "\x1b[1;33m", // Yellow
    [ConsoleThemeStyle.LevelVerbose] = "\x1b[1;30m", // Gray
    [ConsoleThemeStyle.LevelDebug] = "\x1b[1;34m", // Blue
    [ConsoleThemeStyle.LevelInformation] = "\x1b[1;36m", // Cyan
    [ConsoleThemeStyle.LevelWarning] = "\x1b[1;33m", // Yellow
    [ConsoleThemeStyle.LevelError] = "\x1b[1;31m", // Red
    [ConsoleThemeStyle.LevelFatal] = "\x1b[1;31m", // Red
});
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        theme: darkTheme) // Log to console
    .WriteTo.File("c:\\Voteritoas\\logs/log.txt") // Log to a file
    .CreateLogger();
Log.Information("ElectionAuthorityService.Core is starting.");

#endregion

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

    Log.Information("ElectionAuthorityService.Core is starting.");
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
    builder.Services.AddSingleton<WeatherForecastService>();
    builder.Services.AddHostedService<ElectionAuthorityService.Services.ElectionAuthorityService>();
// builder.Services.AddSerilog(Log.Logger)
//     .AddHostedService<Worker>();

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception e)
{
    Log.Logger.Fatal(e, "Fatal error occurred during startup.");
}
