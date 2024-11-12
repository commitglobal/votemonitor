namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class IncidentReportRequest
{
    public string ObserverToken { get; set; }
    public Guid Id { get; set; }
    public string FormId { get; set; }

    public string LocationType { get; set; }
    public Guid? PollingStationId { set; get; }
    public string LocationDescription { get; set; }

    public List<BaseAnswerRequest> Answers { get; set; } = [];

    public override bool Equals(object obj)
    {
        IncidentReportRequest other = (IncidentReportRequest)obj;
        return $"{LocationType}-{PollingStationId}-{LocationDescription}" ==
               $"{other.LocationType}-{other.PollingStationId}-{other.LocationDescription}";
    }

    public override int GetHashCode()
    {
        return $"{LocationType}-{PollingStationId}-{LocationDescription}".GetHashCode();
    }
}