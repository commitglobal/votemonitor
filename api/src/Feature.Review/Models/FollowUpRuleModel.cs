using Vote.Monitor.Domain.Entities.FollowUpAggregate;

namespace Feature.Review.Models;

public record FollowUpRuleModel : AutomationRuleModel
{
    public required Guid FollowUpStatusId { get; init; }

    public static FollowUpRuleModel FromEntity(FollowUpRule rule, IReadOnlyDictionary<Guid, string> userNames) => new()
    {
        Id = rule.Id,
        Name = rule.Name,
        RecordTypes = rule.RecordTypes,
        Conditions = rule.Conditions,
        CreatedAt = rule.CreatedOn,
        UpdatedAt = rule.LastModifiedOn ?? rule.CreatedOn,
        UpdatedById = rule.LastModifiedBy,
        UpdatedByName = userNames.GetValueOrDefault(rule.LastModifiedBy, ""),
        FollowUpStatusId = rule.FollowUpStatusId
    };
}
