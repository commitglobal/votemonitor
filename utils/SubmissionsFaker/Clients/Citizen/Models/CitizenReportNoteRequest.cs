namespace SubmissionsFaker.Clients.Citizen.Models;

public class CitizenReportNoteRequest
{
    public string CitizenReportId { get; set; }
    public string FormId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}