namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class QuickReportRequest
{
    public string ObserverToken { get; set; }
    public Guid Id { get; set; }
    public Guid PollingStationId { set; get; }
    public string QuickReportLocationType { get; set; } = "VisitedPollingStation";
    public string Title { get; set; }
    public string Description { get; set; }
}