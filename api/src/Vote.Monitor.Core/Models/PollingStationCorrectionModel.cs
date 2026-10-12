namespace Vote.Monitor.Core.Models;

/// <summary>A polling station as the correction lists and history show it.</summary>
public record StationRefModel
{
    public Guid Id { get; init; }
    public string Number { get; init; } = string.Empty;
    public string Level1 { get; init; } = string.Empty;
    public string? Level2 { get; init; }
    public string? Level3 { get; init; }
    public string? Level4 { get; init; }
    public string? Level5 { get; init; }
    public string Address { get; init; } = string.Empty;
}

/// <summary>One move of a record to another polling station by a platform admin.</summary>
public record PollingStationCorrectionModel
{
    public DateTime CorrectedAt { get; init; }
    public StationRefModel From { get; init; } = null!;
    public StationRefModel To { get; init; } = null!;
}
