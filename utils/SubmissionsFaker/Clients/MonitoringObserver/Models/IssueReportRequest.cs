namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class IssueReportRequest
{
    public string ObserverToken { get; set; }
    public Guid IssueReportId { get; set; }
    public string FormId { get; set; }

    public string LocationType { get; set; }
    public Guid? PollingStationId { set; get; }
    public string LocationDescription { get; set; }

    public List<BaseAnswerRequest> Answers { get; set; } = [];

    public override bool Equals(object obj)
    {
        IssueReportRequest other = (IssueReportRequest)obj;
        return $"{LocationType}-{PollingStationId}-{LocationDescription}" ==
               $"{other.LocationType}-{other.PollingStationId}-{other.LocationDescription}";
    }

    public override int GetHashCode()
    {
        return $"{LocationType}-{PollingStationId}-{LocationDescription}".GetHashCode();
    }
}