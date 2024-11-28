namespace SubmissionsFaker.Clients.Citizen.Models;

public class CitizenReportNoteRequest
{
    public Guid CitizenReportId { get; set; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}