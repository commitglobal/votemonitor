using System.ComponentModel.DataAnnotations;

namespace Vote.Monitor.Feature.PollingStation.Models;
public  class TagModel
{
    [Key] public int Id { get; set; }

    public required string Key { get; set; }
    public required string Value { get; set; }

    public List<PollingStationModel> PollingStations { get; } = new();
}
