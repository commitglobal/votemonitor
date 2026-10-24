using System.Text.Json.Serialization;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

namespace Feature.Review.Models;

public record ReviewActivityModel
{
    public required Guid Id { get; init; }
    public required ReviewActivityKind Kind { get; init; }

    /// <summary>
    /// The user who made the change; null when an automation rule made it, or when the report went back to the
    /// default stage because its author changed it (<see cref="ReviewActivityKind.ReturnedToDefaultStage"/>).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Guid? ActorId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? ActorName { get; init; }

    /// <summary>The automation rule that made the change, with its name at the time; null when a user made it.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Guid? RuleId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? RuleName { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public AutomationRuleKind? RuleKind { get; init; }

    public required DateTime OccurredAt { get; init; }
    public Guid? FromStageId { get; init; }
    public string? FromStageName { get; init; }
    public Guid? ToStageId { get; init; }
    public string? ToStageName { get; init; }
    public ReviewStatus? ToStatus { get; init; }
    public Guid? ReviewerId { get; init; }
    public string? ReviewerName { get; init; }
    public string? FollowUpStatus { get; init; }
    public SeverityLevel? FromSeverity { get; init; }
    public SeverityLevel? ToSeverity { get; init; }

    public static ReviewActivityModel FromEntity(ReviewActivity activity) => new()
    {
        Id = activity.Id,
        Kind = activity.Kind,
        ActorId = activity.ActorId,
        ActorName = activity.ActorName,
        RuleId = activity.RuleId,
        RuleName = activity.RuleName,
        RuleKind = activity.RuleKind,
        OccurredAt = activity.OccurredAt,
        FromStageId = activity.FromStageId,
        FromStageName = activity.FromStageName,
        ToStageId = activity.ToStageId,
        ToStageName = activity.ToStageName,
        ToStatus = activity.ToStatus,
        ReviewerId = activity.ReviewerId,
        ReviewerName = activity.ReviewerName,
        FollowUpStatus = activity.FollowUpStatus,
        FromSeverity = activity.FromSeverity,
        ToSeverity = activity.ToSeverity
    };
}
