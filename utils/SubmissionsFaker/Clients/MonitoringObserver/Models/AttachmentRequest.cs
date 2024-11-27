namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class AttachmentRequest
{
    public string ObserverToken { get; set; }
    public Guid PollingStationId { get; set; }

    public Guid Id { set; get; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }
}