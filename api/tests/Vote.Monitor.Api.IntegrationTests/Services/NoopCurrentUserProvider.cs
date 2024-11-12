using System.Security.Claims;
using Vote.Monitor.Core.Services.Security;

namespace Vote.Monitor.Api.IntegrationTests.Services;

public class NoopCurrentUserProvider : ICurrentUserProvider
{
    public ClaimsPrincipal? User { get; }
    public Guid? GetUserId()
    {
        throw new NotImplementedException();
    }

    public Guid? GetNgoId()
    {
        throw new NotImplementedException();
    }

    public string? GetClaimValue(string type)
    {
        throw new NotImplementedException();
    }
}
