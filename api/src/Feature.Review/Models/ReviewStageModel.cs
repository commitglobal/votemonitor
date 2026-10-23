namespace Feature.Review.Models;

public record ReviewStageModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required ReviewStatus Status { get; init; }
    public required double Sequence { get; init; }
    public required bool IsDefault { get; init; }

    public static ReviewStageModel FromEntity(ReviewStage stage) => new()
    {
        Id = stage.Id,
        Name = stage.Name,
        Description = stage.Description,
        Status = stage.Status,
        Sequence = stage.Sequence,
        IsDefault = stage.IsDefault
    };
}
