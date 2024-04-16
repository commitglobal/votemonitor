namespace Authorization.Policies.Requirements;

public class MonitoringNgoAdminRequirement(Guid electionRoundId) : IAuthorizationRequirement
{
    public Guid ElectionRoundId { get; } = electionRoundId;
}
