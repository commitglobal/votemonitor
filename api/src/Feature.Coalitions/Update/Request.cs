namespace Feature.NgoCoalitions.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid CoalitionId { get; set; }
    public string CoalitionName { get; set; }
    public Guid[] NgoMembersIds { get; set; } = [];
}
