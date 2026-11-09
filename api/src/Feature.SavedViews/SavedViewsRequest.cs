namespace Feature.SavedViews;

/// <summary>Route and claims shared by the saved view endpoints: the caller's NGO's views in a round.</summary>
public class SavedViewsRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }
}

/// <summary>One of the caller's NGO's views.</summary>
public class SavedViewRequest : SavedViewsRequest
{
    public Guid ViewId { get; set; }
}

/// <summary>The fields of a view as sent when creating or updating it.</summary>
public class SaveViewRequest : SavedViewsRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    /// <summary>Who sees the view; <see cref="SavedViewAccess.Ngo"/> when left out.</summary>
    public SavedViewAccess? Access { get; set; }

    /// <summary>The list's state, a JSON object the web app owns; stored as given.</summary>
    public JsonElement? State { get; set; }
}

public class SavedViewRequestValidator<T> : Validator<T> where T : SavedViewRequest
{
    public SavedViewRequestValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ViewId).NotEmpty();
    }
}

public abstract class SaveViewRequestValidator<T> : Validator<T> where T : SaveViewRequest
{
    protected SaveViewRequestValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("The name is required")
            .Must(x => x is null || x.Trim().Length <= SavedView.MaxNameLength)
            .WithMessage($"The name can have at most {SavedView.MaxNameLength} characters");
        RuleFor(x => x.Description)
            .Must(x => x is null || x.Trim().Length <= SavedView.MaxDescriptionLength)
            .WithMessage($"The description can have at most {SavedView.MaxDescriptionLength} characters");
        RuleFor(x => x.State)
            .Must(x => x is { ValueKind: JsonValueKind.Object })
            .WithMessage("The state must be a JSON object")
            .Must(x => x is null || SavedViewState.Size(x.Value) <= SavedView.MaxStateBytes)
            .WithMessage($"The state can take at most {SavedView.MaxStateBytes / 1024} KB");
    }
}
