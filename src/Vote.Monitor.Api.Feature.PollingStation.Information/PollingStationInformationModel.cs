using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information;

public record PollingStationInformationModel
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }
    public IReadOnlyList<BaseAnswerModel> Answers { get; set; }


    public static PollingStationInformationModel FromEntity(PollingStationInformation entity) => new()
    {
        Id = entity.Id,
        Answers = entity.Answers
            .Select(AnswerMapper.ToModel)
            .ToList(),
        CreatedAt = entity.CreatedOn,
        UpdatedAt = entity.LastModifiedOn
    };
}
