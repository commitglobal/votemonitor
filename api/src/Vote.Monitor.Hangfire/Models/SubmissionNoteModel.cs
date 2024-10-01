namespace Vote.Monitor.Hangfire.Models;

public class SubmissionNoteModel
{
    public Guid QuestionId { get; init; }
    public string Text { get; init; }
}