using System.Security.Claims;

namespace JadeMaui.Services;

public interface IUserManager
{
    public Task LogOutUser();
    public Task<ClaimsPrincipal?> GetAuthenticatedUser();
    public Task<string?> GetUserClaim(string claim, ClaimsPrincipal? user = null);
    public static abstract Task SetAccessToken(string accessToken);
    public static abstract Task SetIdentityToken(string identityToken);
    public static abstract Task SetRefreshToken(string refreshToken);
}
