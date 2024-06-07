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
        LoadingStack.IsVisible= false;

        if (user == null)
        {
            LoginStack.IsVisible = true;
            return;
        }

        UsernameLabel.Text = user.Identity.Name;
        UserEmailLabel.Text = await _userManager.GetUserClaim("email", user);
        UserPicture.Source = await _userManager.GetUserClaim("picture", user);

        LoginStack.IsVisible = false;
        Profile.IsVisible = true;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // If audience parameter is not mentioned, returned token will be an opaque token instead of a JWT.
        var loginResult = await _auth0Client.LoginAsync(new { audience = _configuration.GetValue<string>("Auth0:Audience")});

        if (!loginResult.IsError)
        {
            UsernameLabel.Text = loginResult.User.Identity.Name;
            UserEmailLabel.Text = await _userManager.GetUserClaim("email", loginResult.User);
            UserPicture.Source = await _userManager.GetUserClaim("picture", loginResult.User);

            LoginStack.IsVisible = false;
            Profile.IsVisible = true;

            await UserManager.SetAccessToken(loginResult.AccessToken);
            await UserManager.SetIdentityToken(loginResult.IdentityToken);
            if (loginResult.RefreshToken != null)
                await UserManager.SetRefreshToken(loginResult.RefreshToken);
        }
        else
        {
            await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
        }
    }

    private void Logout(object? sender, EventArgs eventArgs)
    {
        _userManager.LogOutUser();

        Profile.IsVisible = false;
        LoginStack.IsVisible = true;
    }
}
