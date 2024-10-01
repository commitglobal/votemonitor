using System.Text.Json;

namespace Vote.Monitor.Hangfire.Jobs.Export.Locations.ReadModels;

public class LocationModel
{
    public Guid Id { get; set; }
    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string Level4 { get; set; }
    public string Level5 { get; set; }
    public string DisplayOrder { get; set; }
    public JsonDocument Tags { get; set; }
}