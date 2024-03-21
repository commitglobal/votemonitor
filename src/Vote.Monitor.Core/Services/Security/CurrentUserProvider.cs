using System.Security.Claims;

namespace Vote.Monitor.Core.Services.Security;

public class CurrentUserProvider : ICurrentUserProvider, ICurrentUserInitializer
{
    private ClaimsPrincipal? _user;
    private Dictionary<string, IEnumerable<Claim>>? _claimsDict => _user?
        .Claims
        .GroupBy(c => c.Type)
        .ToDictionary(c => c.Key, c => c.Select(v => v));

    public string? Name => _user?.Identity?.Name;

    public Guid? GetUserId()
    {
        if (!IsAuthenticated())
        {
            return null;
        }

        var userIdClaimsValue = GetClaimValue(_claimsDict, "sub");

        if (string.IsNullOrWhiteSpace(userIdClaimsValue))
        {
            return null;
        }

        return Guid.Parse(userIdClaimsValue);
    }

    public Guid? GetNgoId()
    {
        if (!IsAuthenticated())
        {
            return null;
        }

        var ngoIdClaimValue = GetClaimValue(_claimsDict, "ngoId");

        if (string.IsNullOrWhiteSpace(ngoIdClaimValue))
        {
            return null;
        }

        return Guid.Parse(ngoIdClaimValue);
    }

    public bool IsAuthenticated() =>
        _user?.Identity?.IsAuthenticated is true;

    public bool IsInRole(string role) =>
        _user?.IsInRole(role) is true;

    public IEnumerable<Claim>? GetUserClaims() => _user?.Claims;

    public bool IsPlatformAdmin() => _user?.IsInRole("PlatformAdmin") is true;
    public bool IsNgoAdmin() => _user?.IsInRole("NgoAdmin") is true;

    public bool IsObserver() => _user?.IsInRole("Observer") is true;

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        _user = user;
    }

    private string? GetClaimValue(Dictionary<string, IEnumerable<Claim>>? claims, string type)
    {
        if (claims == null)
        {
            return null;
        }

        if (!claims.ContainsKey(type))
        {
            return null;
        }

        return claims[type].FirstOrDefault()?.Value;
    }
}
