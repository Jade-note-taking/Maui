using Auth0.OidcClient;
using JadeMaui.Services;
using Microsoft.Extensions.Configuration;

namespace JadeMaui.Views;

public partial class ProfilePage : ContentPage
{
    private readonly Auth0Client _auth0Client;
    private UserManager _userManager;
    private readonly IConfiguration _configuration;

    public ProfilePage(Auth0Client client, UserManager userManager, IConfiguration configuration)
    {
        InitializeComponent();
        _auth0Client = client;
        _userManager = userManager;
        _configuration = configuration;
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
        // If audience parameter is not mentioned, returned token will be an opaque token instead of a JWT.
        var loginResult = await _auth0Client.LoginAsync(new { audience = _configuration.GetValue<string>("Auth0:Audience")});

        if (!loginResult.IsError)
        {
            UsernameLbl.Text = loginResult.User.Identity.Name;
            UserPictureImg.Source = loginResult.User
              .Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            LoginView.IsVisible = false;
            HomeView.IsVisible = true;
            try
            {
                await UserManager.SetAccessToken(loginResult.AccessToken);
                await UserManager.SetIdentityToken(loginResult.IdentityToken);
                if (loginResult.RefreshToken != null)
                {
                    await UserManager.SetRefreshToken(loginResult.RefreshToken);
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
        _userManager.LogOutUser();

        HomeView.IsVisible = false;
        LoginView.IsVisible = true;
    }
}
