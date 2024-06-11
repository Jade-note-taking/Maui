using System.Security.Claims;
using Auth0.OidcClient;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using Microsoft.IdentityModel.Tokens;

namespace JadeMaui.Services;

public class UserManager : IUserManager
{
    private string _domain = "";
    private string _clientId = "";
    private readonly Auth0Client _client;

    public UserManager(string domain, string clientId, Auth0Client client)
    {
        _domain = domain;
        _clientId = clientId;
        _client = client;
    }

    public static async Task SetAccessToken(string accessToken)
    {
        await SecureStorage.Default.SetAsync("access_token", accessToken);
    }

    public static async Task SetIdentityToken(string identityToken)
    {
        await SecureStorage.Default.SetAsync("id_token", identityToken);
    }

    public static async Task SetRefreshToken(string refreshToken)
    {
        await SecureStorage.Default.SetAsync("refresh_token", refreshToken);
    }

    public async Task<ClaimsPrincipal?> GetAuthenticatedUser()
    {
        ClaimsPrincipal user = null;
        var idToken = await SecureStorage.Default.GetAsync("id_token");

        if (idToken == null) return user;

        var doc = await new HttpClient().GetDiscoveryDocumentAsync($"https://{_domain}");
        var validator = new JwtHandlerIdentityTokenValidator();
        var options = new OidcClientOptions
        {
            ClientId = _clientId,
            ProviderInformation = new ProviderInformation
            {
                IssuerName = doc.Issuer,
                KeySet = doc.KeySet
            }
        };

        try
        {
            var validationResult = await validator.ValidateAsync(idToken, options);
            if (!validationResult.IsError) user = validationResult.User;
            return user;
        }
        catch (SecurityTokenExpiredException)
        {
            var refreshToken = await SecureStorage.Default.GetAsync("refresh_token");
            if (refreshToken == null)
            {
                LogOutUser();
                return null;
            }

            var refreshTokenResult = await _client.RefreshTokenAsync(refreshToken);
            await SetAccessToken(refreshTokenResult.AccessToken);
            await SetIdentityToken(refreshTokenResult.IdentityToken);
            return await GetAuthenticatedUser();
        }
    }

    public async Task LogOutUser()
    {
        await _client.LogoutAsync();

        SecureStorage.Default.RemoveAll();
    }

    public async Task<string?> GetUserClaim(string claim, ClaimsPrincipal? user=null)
    {
        user ??= await GetAuthenticatedUser();

        return user?.FindFirst(c => c.Type == claim)?.Value;
    }
}
