using Vote.Monitor.Core.Models;

namespace Feature.Locations.Services;
public class LocationImportModel
{
    public int DisplayOrder { get; set; }
    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string Level4 { get; set; }
    public string Level5 { get; set; }
    public List<TagImportModel> Tags { get; set; }
}

