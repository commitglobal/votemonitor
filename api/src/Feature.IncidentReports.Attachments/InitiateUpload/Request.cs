namespace Feature.IncidentReports.Attachments.InitiateUpload;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid IncidentReportId { get; set; }

    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public int NumberOfUploadParts { get; set; }
}
