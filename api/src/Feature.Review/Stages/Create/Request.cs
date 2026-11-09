namespace Feature.Review.Stages.Create;

public class Request : StagesRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public ReviewStatus Status { get; set; } = null!;

    /// <summary>Makes it a sub-stage of this top-level stage (it then takes that stage's status).</summary>
    public Guid? ParentStageId { get; set; }

    /// <summary>A key from <c>ReviewStageAppearance.Colors</c>; null for the review status's colour.</summary>
    public string? Color { get; set; }

    /// <summary>A key from <c>ReviewStageAppearance.Icons</c>; null for the review status's icon.</summary>
    public string? Icon { get; set; }
}
