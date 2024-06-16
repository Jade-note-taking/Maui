using System.Security.Claims;

namespace JadeMaui.Services;

public interface IUserManager
{
    Task<ClaimsPrincipal?> GetAuthenticatedUser();
    Task LogOutUser();
    Task<string?> GetUserClaim(string claim, ClaimsPrincipal? user = null);
    Task<string?> GetAccessToken();
    Task<string?> GetIdentityToken();
    Task<string?> GetRefreshToken();
}
