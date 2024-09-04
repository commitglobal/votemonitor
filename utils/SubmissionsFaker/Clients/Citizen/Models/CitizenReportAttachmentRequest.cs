namespace SubmissionsFaker.Clients.Citizen.Models;

public class CitizenReportAttachmentRequest
{
    public string CitizenReportId { get; set; }

    public Guid Id { set; get; }
    public string FormId { get; set; }
    public Guid QuestionId { get; set; }
}