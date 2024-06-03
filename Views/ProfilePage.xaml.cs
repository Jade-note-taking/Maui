using Auth0.OidcClient;
namespace JadeMaui;

public partial class ProfilePage : ContentPage
{
    private readonly Auth0Client _auth0Client;

    public ProfilePage(Auth0Client client)
    {
        InitializeComponent();
        _auth0Client = client;
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
