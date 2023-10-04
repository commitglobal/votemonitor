namespace Vote.Monitor.Feature.PollingStation.CreatePollingStation;
internal class PollingStationCreateRequestDto
{
    public int DisplayOrder { get; set; }
    public required string Address { get; set; }
    public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

    //public PollingStationCreateRequestDto()
    //{
    //    Tags = new Dictionary<string, string>();
    //}

}
