namespace Feature.Review.Models;

public record ReviewerRuleModel : AutomationRuleModel
{
    /// <summary>The stages that trigger the rule; empty when they were all deleted (the rule never runs).</summary>
    public required Guid[] StageIds { get; init; }

    public required Guid[] ReviewerIds { get; init; }

    public static ReviewerRuleModel FromEntity(ReviewerRule rule, IReadOnlyDictionary<Guid, string> userNames) => new()
    {
        Id = rule.Id,
        Name = rule.Name,
        RecordTypes = rule.RecordTypes,
        Conditions = rule.Conditions,
        CreatedAt = rule.CreatedOn,
        UpdatedAt = rule.LastModifiedOn ?? rule.CreatedOn,
        UpdatedById = rule.LastModifiedBy,
        UpdatedByName = userNames.GetValueOrDefault(rule.LastModifiedBy, ""),
        StageIds = rule.StageIds,
        ReviewerIds = rule.ReviewerIds
    };
}
