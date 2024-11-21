namespace SubmissionsFaker.Clients.Citizen.Models;

public class CitizenReportAttachmentRequest
{
    public Guid CitizenReportId { get; set; }

    public Guid Id { set; get; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }
}