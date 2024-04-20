namespace Feature.Form.Submissions.ListByObserver;

public record ObserverSubmissionOverview
{
    public Guid MonitoringObserverId { get; init; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string[] Tags { get; init; } = [];
    public int NumberOfFormsSubmitted { get; init; }
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }
    public int NumberOfUploads { get; set; }
    public int NumberOfNotes { get; set; }
    public DateTime LastActivity { get; init; }
}
