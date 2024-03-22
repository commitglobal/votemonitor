namespace Authorization.Policies.Requirements;

public class MonitoringNgoRequirement(Guid electionRoundId) : IAuthorizationRequirement
{
    public Guid ElectionRoundId { get; } = electionRoundId;
}
