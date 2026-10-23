using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Feature.Review.Models;

/// <summary>A record's review stage and reviewers, as seen by the caller's NGO.</summary>
public record ReviewWorkflowModel
{
    public required ReviewStageModel Stage { get; init; }
    public DateTime? StageChangedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public required ReviewerModel[] Reviewers { get; init; }
}

public record ReviewerModel
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required UserStatus Status { get; init; }
}
