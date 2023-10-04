namespace Vote.Monitor.Feature.PollingStation.Models;
public class PollingStationModel
{
    public int Id { get; set; }
    public required string Address { get; set; }
    public int DisplayOrder { get; set; }

    public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
    //public ICollection<PollingStationTag> Tags { get; set; } = new List<PollingStationTag>();


    //public PollingStationModel()
    //{
    //    Tags = new Dictionary<string, string>();
    //}
}
