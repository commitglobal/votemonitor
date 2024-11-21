namespace Feature.NgoCoalitions.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid CoalitionId { get; set; }
}
