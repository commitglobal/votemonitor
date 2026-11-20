namespace Vote.Monitor.Domain.Entities.DashboardAggregate;

/// <summary>
/// A named page of charts a monitoring NGO builds over its form submissions in a round, shared with every member of the
/// NGO (docs/dashboards-backend.md in the web app). The web app owns <see cref="Charts"/> (each chart's kind, filter and
/// place on the grid); the API stores the JSON array as given and never reads it. The admin who made it is
/// <see cref="AuditableBaseEntity.CreatedBy"/>; <see cref="CreatedByName"/> keeps their name once their account is gone.
/// </summary>
public class Dashboard : AuditableBaseEntity, IAggregateRoot
{
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 500;
    public const int MaxCreatorNameLength = 256;

    /// <summary>The most charts a dashboard can hold.</summary>
    public const int MaxCharts = 50;

    /// <summary>The most bytes a dashboard's charts can take once serialised.</summary>
    public const int MaxChartsBytes = 64 * 1024;

    /// <summary>The most dashboards an NGO can have per round.</summary>
    public const int MaxDashboardsPerRound = 100;

    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public JsonDocument Charts { get; private set; }
    public string CreatedByName { get; private set; }

    /// <summary>The row's version: Postgres's <c>xmin</c>, the concurrency token (concurrent edits contract).</summary>
    public uint Version { get; private set; }

    public static Dashboard Create(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        string? description,
        JsonDocument charts,
        Guid createdBy,
        string createdByName) =>
        new(Guid.NewGuid(), electionRoundId, monitoringNgoId, name, description, charts, createdBy, createdByName);

    public void Update(string name, string? description, JsonDocument charts)
    {
        Name = name;
        Description = description;
        Charts = charts;
    }

    private Dashboard(Guid id,
        Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        string? description,
        JsonDocument charts,
        Guid createdBy,
        string createdByName)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        Name = name;
        Description = description;
        Charts = charts;
        CreatedBy = createdBy;
        CreatedByName = createdByName;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private Dashboard()
    {
    }
#pragma warning restore CS8618
}
