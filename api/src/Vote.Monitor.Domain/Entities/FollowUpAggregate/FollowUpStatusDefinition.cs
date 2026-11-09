using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.Entities.FollowUpAggregate;

/// <summary>
/// A follow-up status a monitoring NGO defines for its reports, e.g. "Needs follow-up", "Called the police",
/// "Resolved". Free-form: a name, an optional description, its own colour and icon (keys from
/// <see cref="ReviewAggregate.ReviewStageAppearance"/>) and an order; each monitoring NGO has exactly one default,
/// the status of every report it hasn't set one for.
/// </summary>
public class FollowUpStatusDefinition : AuditableBaseEntity, IAggregateRoot
{
    /// <summary>Gap between the sequences of neighbouring statuses, so a status can be moved between two others.</summary>
    public const double SequenceStep = 1000;

    public Guid Id { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public MonitoringNgo MonitoringNgo { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    /// <summary>Order, ascending.</summary>
    public double Sequence { get; private set; }

    public bool IsDefault { get; private set; }
    public string? Color { get; private set; }
    public string? Icon { get; private set; }

    /// <summary>
    /// For the three statuses every NGO starts with: the fixed follow-up value (NotApplicable, NeedsFollowUp,
    /// Resolved) older clients and the record's own column still use. Null for the NGO's own statuses.
    /// </summary>
    public string? LegacyStatus { get; private set; }

    public static FollowUpStatusDefinition Create(Guid monitoringNgoId,
        string name,
        string? description,
        double sequence,
        string? color,
        string? icon,
        bool isDefault = false,
        string? legacyStatus = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            MonitoringNgoId = monitoringNgoId,
            Name = name,
            Description = description,
            Sequence = sequence,
            Color = color,
            Icon = icon,
            IsDefault = isDefault,
            LegacyStatus = legacyStatus
        };

    /// <summary>The statuses every monitoring NGO starts with, in order: Not applicable (the default), Needs follow-up, Resolved.</summary>
    public static IEnumerable<FollowUpStatusDefinition> CreateDefaults(Guid monitoringNgoId) =>
    [
        Create(monitoringNgoId, "Not applicable", null, SequenceStep, "gray", "circle", isDefault: true, legacyStatus: "NotApplicable"),
        Create(monitoringNgoId, "Needs follow-up", null, SequenceStep * 2, "yellow", "alert", legacyStatus: "NeedsFollowUp"),
        Create(monitoringNgoId, "Resolved", null, SequenceStep * 3, "green", "check", legacyStatus: "Resolved")
    ];

    public void Update(string name, string? description, double sequence, string? color, string? icon)
    {
        Name = name;
        Description = description;
        Sequence = sequence;
        Color = color;
        Icon = icon;
    }

    public void MarkAsDefault() => IsDefault = true;

    public void UnmarkAsDefault() => IsDefault = false;

#pragma warning disable CS8618 // Required by Entity Framework
    private FollowUpStatusDefinition()
    {
    }
#pragma warning restore CS8618
}
