namespace Vote.Monitor.Feature.PollingStation.Models;
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public ICollection<PollingStationTag> PollingStationTag { get; set; } = new List<PollingStationTag>();
}
