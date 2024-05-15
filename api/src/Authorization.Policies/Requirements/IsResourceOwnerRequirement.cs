using Vote.Monitor.Domain.Entities;

namespace Authorization.Policies.Requirements;

public class IsResourceOwnerRequirement(Guid electionRoundId, AuditableBaseEntity resource) : IAuthorizationRequirement
{
    public Guid ElectionRoundId { get; } = electionRoundId;
    public AuditableBaseEntity Resource { get; } = resource;
}
