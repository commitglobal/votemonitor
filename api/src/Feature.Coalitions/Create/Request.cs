namespace Feature.NgoCoalitions.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public string CoalitionName { get; set; }
    public Guid LeaderId { get; set; }
    public Guid[] NgoMembersIds { get; set; }
}
