namespace Feature.Form.Submissions.ListEntries;

public record FormSubmissionEntry
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public string FormCode { get; init; }
    // TODO: extract to smart enum ?
    public string FormType { get; init; } = default!;

    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = default!;
    public string Level2 { get; init; } = default!;
    public string Level3 { get; init; } = default!;
    public string Level4 { get; init; } = default!;
    public string Level5 { get; init; } = default!;
    public string Number { get; init; } = default!;
    public Guid MonitoringObserverId { get; init; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string[] Tags { get; init; }
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }
    public int MediaFilesCount { get; init; }
    public int NotesCount { get; init; }
}
