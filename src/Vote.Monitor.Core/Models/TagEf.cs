using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vote.Monitor.Core.Models;
public  class TagEf
{
    [Key] public int Id { get; set; }

    public required string Key { get; set; }
    public required string Value { get; set; }

    public List<PollingStationEf> PollingStations { get; } = new();
}

public static class TagEfExtensions
{
    public static bool HasTag(this PollingStationEf pollingStation, string key, string value)
    {
        return pollingStation.Tags.Any(t => t.Key == key && t.Value == value);
    }
    public static Dictionary<string,string> TagsDictionary(this PollingStationEf pollingStation)
    {
        return pollingStation.Tags.ToDictionary(t => t.Key, t => t.Value);
    }
    
}
