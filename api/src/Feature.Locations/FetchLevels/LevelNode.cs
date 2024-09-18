using System.Text.Json.Serialization;

namespace Feature.Locations.FetchLevels;

public class LevelNode
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Depth { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ParentId { get; set; }
}
