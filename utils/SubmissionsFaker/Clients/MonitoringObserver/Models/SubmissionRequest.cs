namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class SubmissionRequest
{
    public Guid PollingStationId { get; set; }

    public Guid FormId { get; set; }

    public Guid SubmissionId { get; set; }

    public List<BaseAnswerRequest> Answers { get; set; } = [];
}