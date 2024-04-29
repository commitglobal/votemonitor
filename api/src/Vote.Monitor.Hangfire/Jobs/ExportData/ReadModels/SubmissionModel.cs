using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Hangfire.Jobs.ExportData.ReadModels;

public class SubmissionModel
{
    public Guid SubmissionId { get; init; }
    public Guid FormId { get; set; }
    public DateTime TimeSubmitted { get; init; }
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
    public string Email { get; init; } = default!;
    public string PhoneNumber { get; init; } = default!;
    public BaseAnswer[] Answers { get; init; }
    public NoteModel[] Notes { get; init; }
    public AttachmentModel[] Attachments { get; init; }
}
