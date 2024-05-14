namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class SubmissionRequest
{
    public string ObserverToken { get; set; }
    public Guid PollingStationId { get; set; }

    public string FormId { get; set; }
    public List<BaseAnswerRequest> Answers { get; set; } = [];


    public override bool Equals(object obj)
    {
        SubmissionRequest other = (SubmissionRequest)obj;
        return PollingStationId == other.PollingStationId && FormId == other.FormId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PollingStationId, FormId);
    }
}