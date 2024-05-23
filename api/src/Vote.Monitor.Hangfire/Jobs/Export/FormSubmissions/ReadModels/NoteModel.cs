namespace Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;

public class NoteModel
{
    public Guid QuestionId { get; init; }
    public string Text { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid MonitoringObserverId { get; set; }
}
