using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// One change to a report's workflow, as seen by one monitoring NGO, made by a user or by one of the NGO's automation
/// rules. Append-only and never cleaned up. Names are copied at the time of the change, so the history still reads
/// correctly after a stage or rule is renamed, a rule is deleted or a user leaves.
/// </summary>
public class ReviewActivity : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public ReportRecordType RecordType { get; private set; }
    public Guid RecordId { get; private set; }
    public ReviewActivityKind Kind { get; private set; }

    /// <summary>
    /// The user who made the change; null when an automation rule made it, or for
    /// <see cref="ReviewActivityKind.ReturnedToDefaultStage"/>.
    /// </summary>
    public Guid? ActorId { get; private set; }
    public string? ActorName { get; private set; }

    /// <summary>The automation rule that made the change; null when a user made it.</summary>
    public Guid? RuleId { get; private set; }
    public string? RuleName { get; private set; }
    public AutomationRuleKind? RuleKind { get; private set; }

    public DateTime OccurredAt { get; private set; }

    public Guid? FromStageId { get; private set; }
    public string? FromStageName { get; private set; }
    public Guid? ToStageId { get; private set; }
    public string? ToStageName { get; private set; }
    public ReviewStatus? ToStatus { get; private set; }

    public Guid? ReviewerId { get; private set; }
    public string? ReviewerName { get; private set; }

    /// <summary>For <see cref="ReviewActivityKind.FollowUpStatusChanged"/>: the new follow-up status value.</summary>
    public string? FollowUpStatus { get; private set; }

    /// <summary>For <see cref="ReviewActivityKind.SeverityChanged"/>: the severity before and after the change.</summary>
    public SeverityLevel? FromSeverity { get; private set; }
    public SeverityLevel? ToSeverity { get; private set; }

    public static ReviewActivity StageChanged(ReviewTarget target,
        ReviewActor actor,
        ReviewStage? from,
        ReviewStage to,
        DateTime occurredAt) =>
        new(target, ReviewActivityKind.StageChanged, actor, occurredAt)
        {
            FromStageId = from?.Id,
            FromStageName = from?.Name,
            ToStageId = to.Id,
            ToStageName = to.Name,
            ToStatus = to.Status
        };

    /// <summary>The report's author changed it, which sends it back to the NGO's default stage.</summary>
    public static ReviewActivity ReturnedToDefaultStage(ReviewTarget target,
        ReviewStage from,
        ReviewStage to,
        DateTime occurredAt) =>
        new(target, ReviewActivityKind.ReturnedToDefaultStage, occurredAt)
        {
            FromStageId = from.Id,
            FromStageName = from.Name,
            ToStageId = to.Id,
            ToStageName = to.Name,
            ToStatus = to.Status
        };

    public static ReviewActivity ReviewerAdded(ReviewTarget target,
        ReviewActor actor,
        Guid reviewerId,
        string reviewerName,
        DateTime occurredAt) =>
        new(target, ReviewActivityKind.ReviewerAdded, actor, occurredAt)
        {
            ReviewerId = reviewerId,
            ReviewerName = reviewerName
        };

    public static ReviewActivity ReviewerAdded(ReviewTarget target,
        AutomationRule rule,
        Guid reviewerId,
        string reviewerName,
        DateTime occurredAt) =>
        new(target, ReviewActivityKind.ReviewerAdded, rule, occurredAt)
        {
            ReviewerId = reviewerId,
            ReviewerName = reviewerName
        };

    public static ReviewActivity ReviewerRemoved(ReviewTarget target,
        ReviewActor actor,
        Guid reviewerId,
        string reviewerName,
        DateTime occurredAt) =>
        new(target, ReviewActivityKind.ReviewerRemoved, actor, occurredAt)
        {
            ReviewerId = reviewerId,
            ReviewerName = reviewerName
        };

    public static ReviewActivity FollowUpStatusChanged(ReviewTarget target,
        ReviewActor actor,
        string followUpStatus,
        DateTime occurredAt) =>
        new(target, ReviewActivityKind.FollowUpStatusChanged, actor, occurredAt)
        {
            FollowUpStatus = followUpStatus
        };

    public static ReviewActivity SeverityChanged(ReviewTarget target,
        ReviewActor actor,
        SeverityLevel from,
        SeverityLevel to,
        DateTime occurredAt) =>
        new(target, ReviewActivityKind.SeverityChanged, actor, occurredAt)
        {
            FromSeverity = from,
            ToSeverity = to
        };

    public static ReviewActivity SeverityChanged(ReviewTarget target,
        AutomationRule rule,
        SeverityLevel from,
        SeverityLevel to,
        DateTime occurredAt) =>
        new(target, ReviewActivityKind.SeverityChanged, rule, occurredAt)
        {
            FromSeverity = from,
            ToSeverity = to
        };

    private ReviewActivity(ReviewTarget target, ReviewActivityKind kind, ReviewActor actor, DateTime occurredAt)
        : this(target, kind, occurredAt)
    {
        ActorId = actor.Id;
        ActorName = actor.Name;
    }

    private ReviewActivity(ReviewTarget target, ReviewActivityKind kind, AutomationRule rule, DateTime occurredAt)
        : this(target, kind, occurredAt)
    {
        RuleId = rule.Id;
        RuleName = rule.Name;
        RuleKind = rule.Kind;
    }

    private ReviewActivity(ReviewTarget target, ReviewActivityKind kind, DateTime occurredAt)
    {
        Id = Guid.NewGuid();
        ElectionRoundId = target.ElectionRoundId;
        MonitoringNgoId = target.MonitoringNgoId;
        RecordType = target.RecordType;
        RecordId = target.RecordId;
        Kind = kind;
        OccurredAt = occurredAt;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private ReviewActivity()
    {
    }
#pragma warning restore CS8618
}

/// <summary>A report as seen by one monitoring NGO: whose workflow a change belongs to.</summary>
public sealed record ReviewTarget(
    Guid ElectionRoundId,
    Guid MonitoringNgoId,
    ReportRecordType RecordType,
    Guid RecordId);

/// <summary>The user making a workflow change, with the name to keep in the history.</summary>
public sealed record ReviewActor(Guid Id, string Name);
