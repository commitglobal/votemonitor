namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class QuickReportAttachmentRequest
{
    public string ObserverToken { get; set; }
    public Guid QuickReportId { get; set; }
    public Guid Id { get; set; }


    public override bool Equals(object obj)
    {
        QuickReportAttachmentRequest other = (QuickReportAttachmentRequest)obj;
        return ObserverToken == other.ObserverToken && QuickReportId == other.QuickReportId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ObserverToken, QuickReportId);
    }
}