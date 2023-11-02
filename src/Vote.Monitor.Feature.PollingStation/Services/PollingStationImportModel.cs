namespace Vote.Monitor.Feature.PollingStation.Services;
public class PollingStationImportModel
{
    public int DisplayOrder { get; set; }
    public string Address { get; set; }
    public List<TagImportModel> Tags { get; set; }
}

public class TagImportModel
{
    public string? Name { get; set; }
    public string? Value { get; set; }
}
