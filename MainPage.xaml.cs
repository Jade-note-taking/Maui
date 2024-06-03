using Auth0.OidcClient;
using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui;

public partial class MainPage : ContentPage
{
    private readonly Auth0Client _auth0Client;
    private readonly HubConnection _connection;

    public MainPage(Auth0Client client, HubConnection connection)
    {
        InitializeComponent();
        _auth0Client = client;
        _connection = connection;

        Task.Run(() =>
        {
            // Connecting to Hub
            Dispatcher.Dispatch(async () => await _connection.StartAsync());

            // Listening
            _connection.On<string>("Note.Create", (message) =>
            {
                Message.Text = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()} - there's new message coming!";
            });
        });
    }

    private async void  CreateNewNote(object sender, EventArgs e)
    {
        await _connection.InvokeCoreAsync("Create", args: new object?[] { "my note name", "my start location" });
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var loginResult = await _auth0Client.LoginAsync();

        if (!loginResult.IsError)
        {
            UsernameLbl.Text = loginResult.User.Identity.Name;
            UserPictureImg.Source = loginResult.User
              .Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            LoginView.IsVisible = false;
            HomeView.IsVisible = true;
        }
        else
        {
            await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var logoutResult = await _auth0Client.LogoutAsync();

        HomeView.IsVisible = false;
        LoginView.IsVisible = true;
    }
}
