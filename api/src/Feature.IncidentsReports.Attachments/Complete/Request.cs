namespace Feature.IncidentsReports.Attachments.Complete;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid IncidentReportId { get; set; }

    public Guid Id { get; set; }
    public string UploadId { get; set; }
    public Dictionary<int, string> Etags { get; set; }
}
