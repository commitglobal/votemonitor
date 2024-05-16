using System.Security.Claims;
using Vote.Monitor.Core.Security;

namespace Authorization.Policies;

public class CurrentUserProvider : ICurrentUserProvider, ICurrentUserInitializer
{
    public ClaimsPrincipal? User { get; private set; }

    private Dictionary<string, IEnumerable<Claim>>? _claimsDict => User?
        .Claims
        .GroupBy(c => c.Type)
        .ToDictionary(c => c.Key, c => c.AsEnumerable());

    public bool IsAuthenticated() => User?.Identity?.IsAuthenticated is true;

    public Guid? GetUserId()
    {
        if (!IsAuthenticated())
        {
            return null;
        }

        var userIdClaimsValue = GetClaimValue(ApplicationClaimTypes.UserId);

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

        var ngoIdClaimValue = GetClaimValue(ApplicationClaimTypes.NgoId);

        if (!string.IsNullOrWhiteSpace(ngoIdClaimValue))
        {
            return Guid.Parse(ngoIdClaimValue);
        }

        return null;
    }

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (User != null)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        User = user;
    }

    public string? GetClaimValue(string type)
    {
        if (_claimsDict == null)
        {
            return null;
        }

        if (_claimsDict.TryGetValue(type, out var values))
        {
            return values.FirstOrDefault()?.Value;
        }

        return null;
    }
}
