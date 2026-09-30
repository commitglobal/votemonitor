using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Hangfire.Jobs.Export.PollingStationInformation.ReadModels;

public class PsiSubmissionModel
{
    public Guid SubmissionId { get; init; }
    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = string.Empty;
    public string Level2 { get; init; } = string.Empty;
    public string Level3 { get; init; } = string.Empty;
    public string Level4 { get; init; } = string.Empty;
    public string Level5 { get; init; } = string.Empty;
    public string Number { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string NgoName { get; init; } = string.Empty;
    public DateTime? ArrivalTime { get; init; }
    public DateTime? DepartureTime { get; init; }
    public double MinutesMonitoring { get; init; }
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public BaseAnswer[] Answers { get; init; } = [];
}
