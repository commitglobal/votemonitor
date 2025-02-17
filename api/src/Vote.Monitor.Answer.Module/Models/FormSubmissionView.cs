using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Form.Module.Models;

namespace Vote.Monitor.Answer.Module.Models;

public class FormSubmissionView
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public string FormCode { get; init; }
    public string DefaultLanguage { get; init; }
    public FormType FormType { get; init; } = default!;

    public SubmissionFollowUpStatus FollowUpStatus { get; init; } = default!;

    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = default!;
    public string Level2 { get; init; } = default!;
    public string Level3 { get; init; } = default!;
    public string Level4 { get; init; } = default!;
    public string Level5 { get; init; } = default!;
    public string Number { get; init; } = default!;
    public Guid MonitoringObserverId { get; init; }
    public bool IsOwnObserver { get; init; }
    public string ObserverName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? PhoneNumber { get; init; } = default!;
    public string[] Tags { get; init; } = [];
    public string NgoName { get; init; } = default!;
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
