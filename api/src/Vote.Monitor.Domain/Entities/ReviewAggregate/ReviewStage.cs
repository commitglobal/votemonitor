using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// A workflow stage a monitoring NGO defines for its reports, e.g. "Not reviewed", "Called the observer", "Reviewed".
/// Every stage belongs to a fixed <see cref="ReviewStatus"/>; each monitoring NGO has exactly one default stage.
/// A stage can have sub-stages (one level deep), which share its status; it can have its own colour and icon, each a
/// key from <see cref="ReviewStageAppearance"/>.
/// </summary>
public class ReviewStage : AuditableBaseEntity, IAggregateRoot
{
    /// <summary>Gap between the sequences of neighbouring stages, so a stage can be moved between two others.</summary>
    public const double SequenceStep = 1000;

    public Guid Id { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public MonitoringNgo MonitoringNgo { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public ReviewStatus Status { get; private set; }

    /// <summary>Order within the status, ascending.</summary>
    public double Sequence { get; private set; }

    /// <summary>The stage of every report the monitoring NGO has not moved yet.</summary>
    public bool IsDefault { get; private set; }

    /// <summary>The stage this one is a sub-stage of; null for a top-level stage.</summary>
    public Guid? ParentStageId { get; private set; }

    /// <summary>A key from <see cref="ReviewStageAppearance.Colors"/>; null shows the review status's colour.</summary>
    public string? Color { get; private set; }

    /// <summary>A key from <see cref="ReviewStageAppearance.Icons"/>; null shows the review status's icon.</summary>
    public string? Icon { get; private set; }

    public static ReviewStage Create(Guid monitoringNgoId,
        string name,
        string? description,
        ReviewStatus status,
        double sequence,
        bool isDefault = false,
        Guid? parentStageId = null,
        string? color = null,
        string? icon = null) =>
        new(Guid.NewGuid(), monitoringNgoId, name, description, status, sequence, isDefault)
        {
            ParentStageId = parentStageId,
            Color = color,
            Icon = icon
        };

    /// <summary>The stages every monitoring NGO starts with: Not reviewed (the default), In review and Reviewed.</summary>
    public static IEnumerable<ReviewStage> CreateDefaults(Guid monitoringNgoId) =>
    [
        Create(monitoringNgoId, "Not reviewed", null, ReviewStatus.NotReviewed, SequenceStep, isDefault: true),
        Create(monitoringNgoId, "In review", null, ReviewStatus.InReview, SequenceStep),
        Create(monitoringNgoId, "Reviewed", null, ReviewStatus.Reviewed, SequenceStep)
    ];

    public void Update(string name, string? description, ReviewStatus status, double sequence,
        Guid? parentStageId = null, string? color = null, string? icon = null)
    {
        Name = name;
        Description = description;
        Status = status;
        Sequence = sequence;
        ParentStageId = parentStageId;
        Color = color;
        Icon = icon;
    }

    /// <summary>A sub-stage follows its parent's review status.</summary>
    public void MoveToStatus(ReviewStatus status) => Status = status;

    public void MarkAsDefault() => IsDefault = true;

    public void UnmarkAsDefault() => IsDefault = false;

    private ReviewStage(Guid id,
        Guid monitoringNgoId,
        string name,
        string? description,
        ReviewStatus status,
        double sequence,
        bool isDefault)
    {
        Id = id;
        MonitoringNgoId = monitoringNgoId;
        Name = name;
        Description = description;
        Status = status;
        Sequence = sequence;
        IsDefault = isDefault;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private ReviewStage()
    {
    }
#pragma warning restore CS8618
}
