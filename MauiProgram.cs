using System.Reflection;
using Auth0.OidcClient;
using JadeMaui.Services;
using JadeMaui.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<ProfilePage>();

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
            Scope = "openid profile email offline_access"
        }));

        builder.Services.AddSingleton(new UserManager(builder.Configuration["Auth0:Domain"], builder.Configuration["Auth0:ClientId"]));
        builder.Services.AddSingleton<TokenHandler>();

        builder.Services.AddSingleton(new HubConnectionBuilder()
            .WithUrl(builder.Configuration["SignalR:Url"])
            .Build());

        return builder.Build();
    }
}
