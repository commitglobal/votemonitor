using Vote.Monitor.Domain.Entities.FollowUpAggregate;

namespace Feature.Review.FollowUpStatuses;

public record FollowUpStatusModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required double Sequence { get; init; }
    public required bool IsDefault { get; init; }
    public string? Color { get; init; }
    public string? Icon { get; init; }

    public static FollowUpStatusModel FromEntity(FollowUpStatusDefinition status) => new()
    {
        Id = status.Id,
        Name = status.Name,
        Description = status.Description,
        Sequence = status.Sequence,
        IsDefault = status.IsDefault,
        Color = status.Color,
        Icon = status.Icon
    };
}

/// <summary>Route and claims shared by the follow-up status endpoints: the caller's NGO's statuses in a round.</summary>
public class FollowUpStatusesRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}

/// <summary>What a status is: its name, an optional description and its own colour and icon from the presets.</summary>
public class FollowUpStatusBody : FollowUpStatusesRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
}

public class FollowUpStatusBodyValidator<T> : Validator<T> where T : FollowUpStatusBody
{
    public FollowUpStatusBodyValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Description).MaximumLength(256);
        RuleFor(x => x.Color)
            .Must(color => color is null || ReviewStageAppearance.Colors.Contains(color))
            .WithMessage("Pick one of the preset colours");
        RuleFor(x => x.Icon)
            .Must(icon => icon is null || ReviewStageAppearance.Icons.Contains(icon))
            .WithMessage("Pick one of the preset icons");
    }
}
