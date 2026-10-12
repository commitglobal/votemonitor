namespace Feature.PollingStationCorrections.Models;

/// <summary>Metadata of a record a platform admin can move to another polling station. Never what was reported.</summary>
public record CorrectableRecordModel
{
    public required Guid Id { get; init; }
    public required string RecordType { get; init; }
    public required string ObserverName { get; init; }
    public required string NgoName { get; init; }
    public required string? FormCode { get; init; }
    public required string? FormName { get; init; }
    public required DateTime SubmittedAt { get; init; }
    public required StationRefModel PollingStation { get; init; }
    public required PollingStationCorrectionModel[] Corrections { get; init; }
}
