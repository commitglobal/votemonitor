namespace SubmissionsFaker.Models;

public class CoalitionModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid LeaderId { get; set; }
    public string LeaderName { get; set; } = string.Empty;

    public int NumberOfMembers => Members.Length;
    public CoalitionMember[] Members { get; init; } = [];
}
