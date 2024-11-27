namespace Vote.Monitor.Core.Models;

public class NoteModel
{
    public Guid QuestionId { get; init; }
    public Guid SubmissionId { get; init; }
    public string Text { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid MonitoringObserverId { get; init; }
}
