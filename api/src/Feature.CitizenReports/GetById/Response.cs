using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Feature.CitizenReports.Models;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Form.Module.Models;

namespace Feature.CitizenReports.GetById;

public class Response
{
    public Guid CitizenReportId { get; set; }
    public DateTime TimeSubmitted { get; set; }
    public Guid FormId { get; set; }
    public string FormCode { get; set; }
    public TranslatedString FormName { get; set; }
    public string FormDefaultLanguage { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportFollowUpStatus, string>))]
    public CitizenReportFollowUpStatus FollowUpStatus { get; init; }

    public BaseQuestionModel[] Questions { get; init; } = [];
    public BaseAnswerModel[] Answers { get; init; } = [];
    public NoteModel[] Notes { get; init; } = [];
    public AttachmentModel[] Attachments { get; init; } = [];
}