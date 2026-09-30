namespace Feature.Statistics.GetCoalitionStatistics;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid CoalitionId { get; set; }
}
