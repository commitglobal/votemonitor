using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Hangfire.Models;

namespace Vote.Monitor.Hangfire.Jobs.Export.CitizenReports.ReadModels;

public class CitizenReportModel
{
    public Guid CitizenReportId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportFollowUpStatus, string>))]
    public CitizenReportFollowUpStatus FollowUpStatus { get; init; } = CitizenReportFollowUpStatus.NotApplicable;

    public Guid FormId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public string Level1 { get; init; } = default!;
    public string Level2 { get; init; } = default!;
    public string Level3 { get; init; } = default!;
    public string Level4 { get; init; } = default!;
    public string Level5 { get; init; } = default!;
    public BaseAnswer[] Answers { get; init; } = [];
    public SubmissionNoteModel[] Notes { get; init; } = [];
    public SubmissionAttachmentModel[] Attachments { get; init; } = [];
}