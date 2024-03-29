namespace Vote.Monitor.Api.Feature.PollingStation.FetchAllV3;

public class PollingStationModelV3
{
    public required Guid Id { get; set; }
    public required int? Level1Id { get; set; }
    public required int? Level2Id { get; set; }
    public required int? Level3Id { get; set; }
    public required int? Level4Id { get; set; }
    public required int? Level5Id { get; set; }
    public required string Number { get; set; }
    public required string Address { get; set; }
    public required int DisplayOrder { get; set; }
}
