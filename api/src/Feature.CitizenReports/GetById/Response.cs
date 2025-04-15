using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Module.Answers.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Module.Forms.Models;
using AttachmentModel = Feature.CitizenReports.Models.AttachmentModel;
using NoteModel = Feature.CitizenReports.Models.NoteModel;

namespace Feature.CitizenReports.GetById;

public class Response
{
    public Guid CitizenReportId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid FormId { get; init; }
    public string FormCode { get; init; }
    public TranslatedString FormName { get; init; }
    public string FormDefaultLanguage { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportFollowUpStatus, string>))]
    public CitizenReportFollowUpStatus FollowUpStatus { get; init; }

    public string LocationId { get; init; }
    public string LocationLevel1 { get; init; }
    public string LocationLevel2 { get; init; }
    public string LocationLevel3 { get; init; }
    public string LocationLevel4 { get; init; }
    public string LocationLevel5 { get; init; }

    public BaseQuestionModel[] Questions { get; init; } = [];
    public BaseAnswerModel[] Answers { get; init; } = [];
    public NoteModel[] Notes { get; init; } = [];
    public AttachmentModel[] Attachments { get; init; } = [];
}
