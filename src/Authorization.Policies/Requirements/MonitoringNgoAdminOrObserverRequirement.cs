namespace Authorization.Policies.Requirements;

public class MonitoringNgoAdminOrObserverRequirement(Guid electionRoundId) : IAuthorizationRequirement
{
    public Guid ElectionRoundId { get; } = electionRoundId;
}
