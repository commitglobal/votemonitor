using System.Security.Claims;
using Vote.Monitor.Core.Services.Security;

namespace Vote.Monitor.Hangfire.Services;

public class MockCurrentUserProvider : ICurrentUserProvider
{
    public ClaimsPrincipal? User => throw new NotImplementedException();
    public Guid? GetUserId() => Guid.Empty;
    public Guid? GetNgoId() => Guid.Empty;
    public string? GetClaimValue(string type) => throw new NotImplementedException();
}
