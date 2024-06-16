using System.Reflection;
using Auth0.OidcClient;
using epj.RouteGenerator;
using JadeMaui.Services;
using JadeMaui.Views;
using MauiIcons.Material;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JadeMaui;

[AutoRoutes("Page")]
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
    #if DEBUG
        builder.Logging.AddDebug();
    #endif

        builder
            .UseMauiApp<App>()
            .UseMaterialMauiIcons()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
            });

        // Reading out from assembly into stream, you'll need to re-compile for updating configuration
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("JadeMaui.appsettings.json");
        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
        builder.Configuration.AddConfiguration(config);
        var auth0Domain = builder.Configuration["Auth0:Domain"]!;
        var auth0ClientId = builder.Configuration["Auth0:ClientId"]!;
        var signalRNoteUrl = builder.Configuration["SignalR:NoteUrl"]!;

        // Singletons
        var auth0Client = new Auth0Client(new()
        {
            Domain = auth0Domain,
            ClientId = auth0ClientId,
            RedirectUri = "jade://callback/",
            PostLogoutRedirectUri = "jade://callback/",
            Scope = "openid profile email offline_access"
        });

        // Pages
        builder.Services.AddSingleton<JadePage>();
        builder.Services.AddSingleton<NotesPage>();
        builder.Services.AddSingleton<NotesArchivePage>();
        builder.Services.AddSingleton<ProfilePage>();

        // Services
        builder.Services.AddSingleton<ISignalRService, SignalRService>();
        builder.Services.AddSingleton<INoteService, NoteService>();
        builder.Services.AddSingleton<IDebounceService, DebounceService>();
        builder.Services.AddSingleton<IRestService, RestService>();
        builder.Services.AddSingleton<IUserManager, UserManager>();

        // Other
        builder.Services.AddSingleton(auth0Client);
        builder.Services.AddSingleton(builder.Configuration);
        builder.Services.AddSingleton(new UserManager(auth0Domain, auth0ClientId, auth0Client));

        return builder.Build();
    }
}
