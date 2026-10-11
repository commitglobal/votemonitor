namespace Authorization.Policies.Requirements;

public class CitizenReportingNgoAdminOrStaffRequirement(Guid electionRoundId) : IAuthorizationRequirement
{
    public Guid ElectionRoundId { get; } = electionRoundId;
}
