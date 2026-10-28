using Module.Forms.Models;

namespace Feature.PollingStation.Information.Forms;

public record PollingStationInformationFormModel
{
    public Guid Id { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
    public List<BaseQuestionModel> Questions { get; init; } = [];
    public string DefaultLanguage { get; init; }
    public List<string> Languages { get; init; } = [];

    /// <summary>
    /// How many polling station information records the caller's NGO's observers sent, as the forms list counts its
    /// forms; null when an NGO user didn't ask (platform admins, observers).
    /// </summary>
    public int? NumberOfSubmissions { get; init; }

    public static PollingStationInformationFormModel FromEntity(PollingStationInfoFormAggregate entity) => new()
    {
        Id = entity.Id,
        DefaultLanguage = entity.DefaultLanguage,
        Languages = entity.Languages.ToList(),
        CreatedOn = entity.CreatedOn,
        LastModifiedOn = entity.LastModifiedOn,
        Questions = entity.Questions.Select(QuestionsMapper.ToModel).ToList()
    };
}
