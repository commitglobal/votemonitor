namespace Vote.Monitor.Api.Feature.PollingStation.Information;

public record PollingStationInfoModel
{
    public required Guid Id { get; init; }
    public required PollingStationModel PollingStation { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }
}

public record PollingStationModel
{
    public Guid Id { get; set; }
    public string Address { get; set; }
}
