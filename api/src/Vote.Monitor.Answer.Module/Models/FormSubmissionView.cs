using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Form.Module.Models;

namespace Vote.Monitor.Answer.Module.Models;

public record FormSubmissionView
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public string FormCode { get; init; }
    public string DefaultLanguage { get; init; }
    public FormType FormType { get; init; } = null!;

    public SubmissionFollowUpStatus FollowUpStatus { get; init; } = null!;

    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = null!;
    public string Level2 { get; init; } = null!;
    public string Level3 { get; init; } = null!;
    public string Level4 { get; init; } = null!;
    public string Level5 { get; init; } = null!;
    public string Number { get; init; } = null!;
    public Guid MonitoringObserverId { get; init; }
    public bool IsOwnObserver { get; init; }
    public string ObserverName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null!;
    public string[] Tags { get; init; } = [];
    public string NgoName { get; init; } = null!;
    public int NumberOfFlaggedAnswers { get; init; }
    public int NumberOfQuestionsAnswered { get; init; }

    public BaseQuestionModel[] Questions { get; init; }
    public BaseAnswerModel[] Answers { get; init; } = [];
    public NoteModel[] Notes { get; init; } = [];
    public AttachmentModel[] Attachments { get; init; } = [];

    public DateTime? ArrivalTime { get; init; }
    public DateTime? DepartureTime { get; init; }
    public ObservationBreakModel[] Breaks { get; init; } = [];
    public bool IsCompleted { get; init; }
}
