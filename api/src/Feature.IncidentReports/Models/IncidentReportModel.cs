using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.Models;

public class IncidentReportModel
{
    public required Guid Id { get; init; }
    public required Guid FormId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportFollowUpStatus, string>))]
    public IncidentReportFollowUpStatus FollowUpStatus { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportLocationType, string>))]
    public IncidentReportLocationType LocationType { get; init; }

    public Guid? PollingStationId { get; init; }
    public string? LocationDescription { get; init; }

    public IReadOnlyList<BaseAnswerModel> Answers { get; init; }

    public NoteModel[] Notes { get; init; } = [];
    public AttachmentModel[] Attachments { get; init; } = [];
    public DateTime Timestamp { get; set; }

    public static IncidentReportModel FromEntity(IncidentReport entity) => new()
    {
        Id = entity.Id,
        FormId = entity.FormId,
        Answers = entity.Answers
            .Select(AnswerMapper.ToModel)
            .ToList(),
        FollowUpStatus = entity.FollowUpStatus,
        PollingStationId = entity.PollingStationId,
        LocationDescription = entity.LocationDescription,
        LocationType = entity.LocationType,
        Timestamp = entity.LastModifiedOn ?? entity.CreatedOn
    };
}