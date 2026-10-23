namespace Feature.Review.Stages.Create;

public class Request : StagesRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public ReviewStatus Status { get; set; } = null!;
}
