namespace Vote.Monitor.Feature.PollingStation.Models;
public class PollingStationTag
{
    public int Id { get; set; }
    public int PollingStationId { get; set; }
    public PollingStationModel PollingStation { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}
