using System.Security.Claims;
using Vote.Monitor.Core.Services.Security;

namespace Vote.Monitor.Hangfire;

public class MockCurrentUserIdProvider : ICurrentUserIdProvider
{
    public ClaimsPrincipal? User => throw new NotImplementedException();
    public Guid? GetUserId() => Guid.Empty;

    public string? GetClaimValue(string type) => throw new NotImplementedException();
}
