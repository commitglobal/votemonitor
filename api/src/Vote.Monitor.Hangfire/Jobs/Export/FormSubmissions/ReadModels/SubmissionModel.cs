using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Hangfire.Models;

namespace Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;

public class SubmissionModel
{
    public Guid SubmissionId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<SubmissionFollowUpStatus, string>))]
    public SubmissionFollowUpStatus FollowUpStatus { get; set; }

    public Guid FormId { get; set; }
    public DateTime TimeSubmitted { get; init; }
    public string Level1 { get; init; } = default!;
    public string Level2 { get; init; } = default!;
    public string Level3 { get; init; } = default!;
    public string Level4 { get; init; } = default!;
    public string Level5 { get; init; } = default!;
    public string Number { get; init; } = default!;
    public Guid MonitoringObserverId { get; init; }
    public string NgoName { get; init; } = default!;
    public string DisplayName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string PhoneNumber { get; init; } = default!;
    public bool IsCompleted { get; init; } = default!;
    public BaseAnswer[] Answers { get; init; }
    public SubmissionNoteModel[] Notes { get; init; }
    public SubmissionAttachmentModel[] Attachments { get; init; }
}
