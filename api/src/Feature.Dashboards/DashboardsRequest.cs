namespace Feature.Dashboards;

/// <summary>Route and claims shared by the dashboard endpoints: the caller's NGO's dashboards in a round.</summary>
public class DashboardsRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }
}

/// <summary>One of the caller's NGO's dashboards.</summary>
public class DashboardRequest : DashboardsRequest
{
    public Guid DashboardId { get; set; }
}

/// <summary>A dashboard's fields as sent when creating or replacing it.</summary>
public class SaveDashboardRequest : DashboardsRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    /// <summary>The charts, a JSON array the web app owns; stored as given. Left out: no charts.</summary>
    public JsonElement? Charts { get; set; }
}

public class DashboardRequestValidator<T> : Validator<T> where T : DashboardRequest
{
    public DashboardRequestValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DashboardId).NotEmpty();
    }
}

public abstract class SaveDashboardRequestValidator<T> : Validator<T> where T : SaveDashboardRequest
{
    protected SaveDashboardRequestValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("The name is required")
            .Must(x => x is null || x.Trim().Length <= Dashboard.MaxNameLength)
            .WithMessage($"The name can have at most {Dashboard.MaxNameLength} characters");
        RuleFor(x => x.Description)
            .Must(x => x is null || x.Trim().Length <= Dashboard.MaxDescriptionLength)
            .WithMessage($"The description can have at most {Dashboard.MaxDescriptionLength} characters");
        RuleFor(x => x.Charts)
            .Must(x => x is null || x.Value.ValueKind is JsonValueKind.Array or JsonValueKind.Null)
            .WithMessage("The charts must be a JSON array")
            .Must(x => x is not { ValueKind: JsonValueKind.Array } || x.Value.GetArrayLength() <= Dashboard.MaxCharts)
            .WithMessage($"A dashboard can have at most {Dashboard.MaxCharts} charts")
            .Must(x => x is null || DashboardCharts.Size(x.Value) <= Dashboard.MaxChartsBytes)
            .WithMessage($"The charts can take at most {Dashboard.MaxChartsBytes / 1024} KB");
    }
}
