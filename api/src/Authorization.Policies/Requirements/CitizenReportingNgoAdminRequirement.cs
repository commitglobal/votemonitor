namespace Authorization.Policies.Requirements;

public class CitizenReportingNgoAdminRequirement(Guid electionRoundId) : IAuthorizationRequirement
{
    public Guid ElectionRoundId { get; } = electionRoundId;
}