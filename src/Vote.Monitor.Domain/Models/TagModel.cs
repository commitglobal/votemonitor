using System.ComponentModel.DataAnnotations;

namespace Vote.Monitor.Domain.Models;
public  class TagModel
{
    public  TagModel()
    {
    }

    public TagModel(string key, string tagValue)
    {
        Key = key;
        Value = tagValue;
    }

    [Key] public int Id { get; set; }

    public required string Key { get; set; }
    public required string Value { get; set; }
    
    public List<PollingStationModel> PollingStations { get; } = new();
}
