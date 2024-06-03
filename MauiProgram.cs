using System.Reflection;
using Auth0.OidcClient;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace JadeMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<MainPage>();


        // Reading out from assembly into stream, you'll need to re-compile for updating configuration
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("JadeMaui.appsettings.json");

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(config);


        Console.WriteLine(builder.Configuration["Auth0:Domain"] ?? "we are fucked up");

        builder.Services.AddSingleton(new Auth0Client(new()
        {
            Domain = builder.Configuration["Auth0:Domain"],
            ClientId = builder.Configuration["Auth0:ClientId"],
            RedirectUri = "jade://callback/",
            PostLogoutRedirectUri = "jade://callback/",
            Scope = "openid profile email"
        }));

        builder.Services.AddSingleton(new HubConnectionBuilder()
            .WithUrl(builder.Configuration["SignalR:Url"])
            .Build());

        return builder.Build();
    }
}
