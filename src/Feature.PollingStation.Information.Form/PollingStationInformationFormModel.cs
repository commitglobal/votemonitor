using Vote.Monitor.Form.Module.Models;

namespace Feature.PollingStation.Information.Form;

public record PollingStationInformationFormModel
{
    public Guid Id { get; init; }

    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
    public List<BaseQuestionModel> Questions { get; init; } = [];
    public List<string> Languages { get; init; } = [];

    public static PollingStationInformationFormModel FromEntity(PollingStationInfoFormAggregate entity) => new()
    {
        Id = entity.Id,
        Languages = entity.Languages.ToList(),
        CreatedOn = entity.CreatedOn,
        LastModifiedOn = entity.LastModifiedOn,
        Questions = entity.Questions.Select(FormMapper.ToModel).ToList()
    };
}
