using System.Security.Claims;
using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Core.Services.Security;

public class CurrentUserProvider : ICurrentUserProvider, ICurrentUserInitializer
{
    private ClaimsPrincipal? _user;

    public ClaimsPrincipal? User => _user;

    private Dictionary<string, IEnumerable<Claim>>? _claimsDict => _user?
        .Claims
        .GroupBy(c => c.Type)
        .ToDictionary(c => c.Key, c => c.AsEnumerable());

    public Guid? GetUserId()
    {
        if (!IsAuthenticated())
        {
            return null;
        }

        var userIdClaimsValue = GetClaimValue(_claimsDict, ApplicationClaimTypes.UserId);

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

        var ngoIdClaimValue = GetClaimValue(_claimsDict, ApplicationClaimTypes.NgoId);

        if (string.IsNullOrWhiteSpace(ngoIdClaimValue))
        {
            return null;
        }

        return Guid.Parse(ngoIdClaimValue);
    }

    public bool IsAuthenticated() => _user?.Identity?.IsAuthenticated is true;

    public bool IsPlatformAdmin() => _user?.IsInRole(UserRole.PlatformAdmin.Value) is true;

    public bool IsNgoAdmin() => _user?.IsInRole(UserRole.NgoAdmin.Value) is true;

    public bool IsObserver() => _user?.IsInRole(UserRole.Observer.Value) is true;

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

        if (claims.TryGetValue(type, out var values))
        {
            return values.FirstOrDefault()?.Value;
        }

        return null;
    }
}
