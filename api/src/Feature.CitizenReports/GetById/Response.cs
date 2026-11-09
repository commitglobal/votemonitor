using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Module.Answers.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Module.Forms.Models;

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
    public SubmissionAttachmentModel[] Attachments { get; init; } = [];

    /// <summary>The caller NGO's own follow-up status (see /follow-up-statuses); FollowUpStatus is the record's fixed value.</summary>
    public Guid? FollowUpStatusId { get; init; }
    public string? FollowUpStatusName { get; init; }
    public string? FollowUpStatusColor { get; init; }
    public string? FollowUpStatusIcon { get; init; }
}
