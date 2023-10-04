namespace Vote.Monitor.Feature.PollingStation.Models;
internal class PollingStationImportModel
{
    public int DisplayOrder { get; set; }
    public string Address { get; set; }
    public List<TagImportModel> Tags { get; set; }
}
