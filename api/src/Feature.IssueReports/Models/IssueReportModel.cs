using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;

namespace Feature.IssueReports.Models;

public class IssueReportModel
{
    public required Guid Id { get; init; }
    public required Guid FormId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IssueReportFollowUpStatus, string>))]
    public IssueReportFollowUpStatus FollowUpStatus { get; init; }

    public IReadOnlyList<BaseAnswerModel> Answers { get; init; }

    public static IssueReportModel FromEntity(IssueReportAggregate entity) => new()
    {
        Id = entity.Id,
        FormId = entity.FormId,
        Answers = entity.Answers
            .Select(AnswerMapper.ToModel)
            .ToList(),
        FollowUpStatus = entity.FollowUpStatus,
    };
}