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

    public static DashboardModel FromEntity(Dashboard dashboard, string creatorName, bool canEdit) =>
        new()
        {
            Id = dashboard.Id,
            Name = dashboard.Name,
            Description = dashboard.Description,
            Charts = dashboard.Charts.RootElement.Clone(),
            CreatedBy = new DashboardCreatorModel(dashboard.CreatedBy, creatorName),
            CreatedAt = dashboard.CreatedOn,
            UpdatedAt = dashboard.LastModifiedOn ?? dashboard.CreatedOn,
            CanEdit = canEdit
        };
}

public record DashboardsModel(List<DashboardModel> Dashboards);
