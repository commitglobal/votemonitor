using System.Text.Json.Serialization;

namespace Feature.SavedViews.Models;

public record SavedViewCreatorModel(Guid Id, string DisplayName);

public record SavedViewModel
{
    public required Guid Id { get; init; }
    public required ReportRecordType RecordType { get; init; }
    public required string Name { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public required string? Description { get; init; }

    public required SavedViewAccess Access { get; init; }
    public required JsonElement State { get; init; }
    public required SavedViewCreatorModel CreatedBy { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required bool CanEdit { get; init; }
    public required bool CanChangeAccess { get; init; }

    public static SavedViewModel FromEntity(SavedView view, string creatorName, bool canEdit, bool canChangeAccess) =>
        new()
        {
            Id = view.Id,
            RecordType = view.RecordType,
            Name = view.Name,
            Description = view.Description,
            Access = view.Access,
            State = view.State.RootElement.Clone(),
            CreatedBy = new SavedViewCreatorModel(view.CreatedBy, creatorName),
            CreatedAt = view.CreatedOn,
            UpdatedAt = view.LastModifiedOn ?? view.CreatedOn,
            CanEdit = canEdit,
            CanChangeAccess = canChangeAccess
        };
}

public record SavedViewsModel(List<SavedViewModel> Views);
