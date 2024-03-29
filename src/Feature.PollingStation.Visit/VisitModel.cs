
namespace Feature.PollingStation.Visit;

public record VisitModel
{
    public Guid PollingStationId { get; set; }
    public DateTime VisitedAt { get; set; }
}
