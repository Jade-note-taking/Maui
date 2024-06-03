using Auth0.OidcClient;
using JadeMaui.Services;

namespace JadeMaui.Views;

public partial class ProfilePage : ContentPage
{
    private readonly Auth0Client _auth0Client;
    private UserManager _userManager;

    public ProfilePage(Auth0Client client, UserManager userManager)
    {
        InitializeComponent();
        _auth0Client = client;
        _userManager = userManager;
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        var user = await _userManager.GetAuthenticatedUser();

        if (user == null) return;

        
        // var claimsIdentities = user.Identities;

        UsernameLbl.Text = user.Identity.Name;
        UserPictureImg.Source = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

        LoginView.IsVisible = false;
        HomeView.IsVisible = true;
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
            try
            {
                await SecureStorage.Default.SetAsync("access_token", loginResult.AccessToken);
                await SecureStorage.Default.SetAsync("id_token", loginResult.IdentityToken);
                if (loginResult.RefreshToken != null)
                {
                    await SecureStorage.Default.SetAsync("refresh_token", loginResult.RefreshToken);
                }
            } catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        else
        {
            await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var logoutResult = await _auth0Client.LogoutAsync();

        SecureStorage.Default.RemoveAll();

        HomeView.IsVisible = false;
        LoginView.IsVisible = true;
    }
}
