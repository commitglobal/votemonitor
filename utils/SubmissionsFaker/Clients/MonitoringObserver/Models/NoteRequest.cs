namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class NoteRequest
{
    public string ObserverToken { get; set; }
    public Guid PollingStationId { get; set; }
    public string FormId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}