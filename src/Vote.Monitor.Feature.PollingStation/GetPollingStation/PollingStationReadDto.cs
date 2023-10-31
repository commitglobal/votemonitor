namespace Vote.Monitor.Feature.PollingStation.GetPollingStation;
public class PollingStationReadDto
{
    public Guid Id { get; set; }
    public required string Address { get; set; }
    public int DisplayOrder { get; set; }
    public Dictionary<string, string> Tags { get; set; }

    public PollingStationReadDto()
    {
        Tags = new Dictionary<string, string>();
    }
}
