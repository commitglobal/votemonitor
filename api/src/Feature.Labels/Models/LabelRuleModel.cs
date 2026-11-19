using Feature.Review.Models;

namespace Feature.Labels.Models;

public record LabelRuleModel : AutomationRuleModel
{
    public required string[] Labels { get; init; }

    public static LabelRuleModel FromEntity(LabelRule rule, IReadOnlyDictionary<Guid, string> userNames) => new()
    {
        Id = rule.Id,
        Name = rule.Name,
        RecordTypes = rule.RecordTypes,
        Conditions = rule.Conditions,
        CreatedAt = rule.CreatedOn,
        UpdatedAt = rule.LastModifiedOn ?? rule.CreatedOn,
        UpdatedById = rule.LastModifiedBy,
        UpdatedByName = userNames.GetValueOrDefault(rule.LastModifiedBy, ""),
        Labels = rule.Labels
    };
}
