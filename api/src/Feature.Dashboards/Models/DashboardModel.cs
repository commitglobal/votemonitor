using System.Text.Json.Serialization;

namespace Feature.Dashboards.Models;

public record DashboardCreatorModel(Guid Id, string DisplayName);

public record DashboardModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public required string? Description { get; init; }

    public required JsonElement Charts { get; init; }
    public required DashboardCreatorModel CreatedBy { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }

    /// <summary>Whether the caller may change and delete it (NGO admins).</summary>
    public required bool CanEdit { get; init; }

    /// <summary>The record's version (opaque); sent back with a save (concurrent edits contract).</summary>
    public required uint Version { get; init; }

    /// <summary>When this version was saved (the same as <see cref="UpdatedAt"/>).</summary>
    public required DateTime LastModifiedOn { get; init; }

    /// <summary>The display name of who saved this version; null when unknown.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public required string? LastModifiedByName { get; init; }

    public static DashboardModel FromEntity(Dashboard dashboard, string creatorName, string? lastModifiedByName,
        bool canEdit) =>
        new()
        {
            Id = dashboard.Id,
            Name = dashboard.Name,
            Description = dashboard.Description,
            Charts = dashboard.Charts.RootElement.Clone(),
            CreatedBy = new DashboardCreatorModel(dashboard.CreatedBy, creatorName),
            CreatedAt = dashboard.CreatedOn,
            UpdatedAt = dashboard.LastModifiedOn ?? dashboard.CreatedOn,
            CanEdit = canEdit,
            Version = dashboard.Version,
            LastModifiedOn = dashboard.LastModifiedOn ?? dashboard.CreatedOn,
            LastModifiedByName = lastModifiedByName
        };
}

public record DashboardsModel(List<DashboardModel> Dashboards);
