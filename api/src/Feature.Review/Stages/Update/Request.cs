namespace Feature.Review.Stages.Update;

public class Request : StagesRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public ReviewStatus Status { get; set; } = null!;

    /// <summary>The position within the status; when left out, a stage moved to another status goes last there.</summary>
    public double? Sequence { get; set; }
}
