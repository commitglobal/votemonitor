namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class PSISubmissionRequest
{

    public DateTime ArrivalTime { get; set; }
    public DateTime DepartureTime { get; set; }

    public string ObserverToken { get; set; }
    public string PollingStationId { get; set; }

    public List<BaseAnswerRequest> Answers { get; set; } = [];


    public override bool Equals(object obj)
    {
        PSISubmissionRequest other = (PSISubmissionRequest)obj;
        return PollingStationId == other.PollingStationId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PollingStationId);
    }
}