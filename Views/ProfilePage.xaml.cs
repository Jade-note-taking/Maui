using System.Diagnostics;
using Auth0.OidcClient;
using JadeMaui.Helpers;
using JadeMaui.Services;
using Microsoft.Extensions.Configuration;

namespace JadeMaui.Views;

public partial class ProfilePage : ContentPage
{
    private readonly Auth0Client _auth0Client = ServiceHelper.GetService<Auth0Client>();
    private readonly UserManager _userManager = ServiceHelper.GetService<UserManager>();
    private readonly ConfigurationManager _configuration = ServiceHelper.GetService<ConfigurationManager>();

    public ProfilePage()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        Debug.WriteLine(await SecureStorage.Default.GetAsync("access_token"));
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
        var loginResult = await _auth0Client.LoginAsync(new { audience = _configuration["Auth0:Audience"]});

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

    private async void Logout(object? sender, EventArgs eventArgs)
    {
        Profile.IsVisible = false;
        LoginStack.IsVisible = false;
        LoadingStack.IsVisible= true;

        await _userManager.LogOutUser();

        LoginStack.IsVisible = true;
        LoadingStack.IsVisible= false;
    }
}
