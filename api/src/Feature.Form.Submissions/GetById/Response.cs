using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Feature.Form.Submissions.Models;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Form.Module.Models;

namespace Feature.Form.Submissions.GetById;

public class Response
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public string FormCode { get; init; }
    public string DefaultLanguage { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; init; } = default!;

    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = default!;
    public string Level2 { get; init; } = default!;
    public string Level3 { get; init; } = default!;
    public string Level4 { get; init; } = default!;
    public string Level5 { get; init; } = default!;
    public string Number { get; init; } = default!;
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string PhoneNumber { get; init; } = default!;
    public string[] Tags { get; init; }
    public BaseQuestionModel[] Questions { get; init; }
    public BaseAnswerModel[] Answers { get; init; }
    public NoteModel[] Notes { get; init; }
    public AttachmentModel[] Attachments { get; init; }
}
