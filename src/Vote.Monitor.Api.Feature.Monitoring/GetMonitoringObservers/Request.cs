namespace Vote.Monitor.Api.Feature.Monitoring.GetMonitoringObservers;
public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }
}
