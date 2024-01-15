namespace Vote.Monitor.Api.Feature.ElectionRound;

public record ElectionRoundModel
{
    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
}
