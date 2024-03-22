namespace Authorization.Policies.Requirements;

public class MonitoringObserverRequirement(Guid electionRoundId) : IAuthorizationRequirement
{
    public Guid ElectionRoundId { get; } = electionRoundId;
}
