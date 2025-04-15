using System.Text.Json.Serialization;

namespace Feature.PollingStations.FetchAll;

public class LocationNode
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Depth { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ParentId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Number { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? PollingStationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DisplayOrder { get; set; }
}
