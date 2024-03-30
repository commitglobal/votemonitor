namespace Vote.Monitor.Domain.ViewModels;

public class PollingStationVisitViewModel
{
    public Guid ElectionRoundId { get; set; }
    public Guid PollingStationId { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public Guid NgoId { get; set; }
    public Guid MonitoringObserverId { get; set; }
    public Guid ObserverId { get; set; }
    public DateTime VisitedAt { get; set; }
}
