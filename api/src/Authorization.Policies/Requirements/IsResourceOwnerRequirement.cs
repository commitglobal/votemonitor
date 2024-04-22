using Vote.Monitor.Domain.Entities;

namespace Authorization.Policies.Requirements;

public class IsResourceOwnerRequirement(AuditableBaseEntity resource) : IAuthorizationRequirement
{
    public AuditableBaseEntity Resource { get; } = resource;
}
