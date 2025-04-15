using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Module.Answers.Mappers;
using Module.Answers.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Feature.CitizenReports.Models;

public class CitizenReportModel
{
    public required Guid Id { get; init; }
    public required Guid FormId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportFollowUpStatus, string>))]
    public CitizenReportFollowUpStatus FollowUpStatus { get; init; }

    public IReadOnlyList<BaseAnswerModel> Answers { get; init; }

    public static CitizenReportModel FromEntity(CitizenReportAggregate entity) => new()
    {
        Id = entity.Id,
        FormId = entity.FormId,
        Answers = entity.Answers
            .Select(AnswerMapper.ToModel)
            .ToList(),
        FollowUpStatus = entity.FollowUpStatus
    };
}