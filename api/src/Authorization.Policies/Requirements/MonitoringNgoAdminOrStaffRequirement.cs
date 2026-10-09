namespace Authorization.Policies.Requirements;

public class MonitoringNgoAdminOrStaffRequirement(Guid electionRoundId) : IAuthorizationRequirement
{
    public Guid ElectionRoundId { get; } = electionRoundId;
}
