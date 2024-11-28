namespace Authorization.Policies.Requirements;

public class CoalitionLeaderRequirement(Guid electionRoundId, Guid coalitionId) : IAuthorizationRequirement
{
    public Guid ElectionRoundId { get; } = electionRoundId;
    public Guid CoalitionId { get; } = coalitionId;
}
