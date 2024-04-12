namespace Vote.Monitor.Domain.ViewModels;

public class PollingStationVisitViewModel
{
    public Guid ElectionRoundId { get; set; }
    public Guid PollingStationId { get; set; }
    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string Level4 { get; set; }
    public string Level5 { get; set; }
    public string Address { get; set; }
    public string Number { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public Guid NgoId { get; set; }
    public Guid MonitoringObserverId { get; set; }
    public Guid ObserverId { get; set; }
    public DateTime VisitedAt { get; set; }
}
