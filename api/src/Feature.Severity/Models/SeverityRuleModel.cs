using Feature.Review.Models;

namespace Feature.Severity.Models;

public record SeverityRuleModel : AutomationRuleModel
{
    public required SeverityLevel Severity { get; init; }

    public static SeverityRuleModel FromEntity(SeverityRule rule, IReadOnlyDictionary<Guid, string> userNames) => new()
    {
        Id = rule.Id,
        Name = rule.Name,
        RecordTypes = rule.RecordTypes,
        Conditions = rule.Conditions,
        CreatedAt = rule.CreatedOn,
        UpdatedAt = rule.LastModifiedOn ?? rule.CreatedOn,
        UpdatedById = rule.LastModifiedBy,
        UpdatedByName = userNames.GetValueOrDefault(rule.LastModifiedBy, ""),
        Severity = rule.Severity
    };
}
