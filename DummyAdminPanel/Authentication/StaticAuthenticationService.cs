using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace DummyAdminPanel.Authentication;

public interface IAuthenticationService
{
    ValueTask<bool> TryAuthenticate(string email, string password, bool remember);
}

public class StaticAuthenticationService : IAuthenticationService
{
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly string _expectedEmail;
    readonly string _expectedPassword;

    public StaticAuthenticationService(IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _expectedEmail = appSettings.Value.UserEmail;
        _expectedPassword = appSettings.Value.UserPassword;
    }

    public async ValueTask<bool> TryAuthenticate(string email, string password, bool remember)
    {
        var areCredsValid = _expectedEmail == email && _expectedPassword == password;

        if (!areCredsValid)
            return false;

        List<Claim> claims = new()
        {
            new(ClaimTypes.Name, email),
        };
        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);
        AuthenticationProperties authProps = new()
        {
            AllowRefresh = true,
            IsPersistent = remember,
        };

        var httpCtx = _httpContextAccessor.HttpContext
                      ?? throw new InvalidOperationException($"Cannot access {nameof(HttpContext)}.");
        await httpCtx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
        return true;
    }
}