namespace Vote.Monitor.Feature.PollingStation.Models;

public static class TagModelExtensions
{
    public static bool HasTag(this PollingStationModel pollingStation, string key, string value)
    {
        return pollingStation.Tags.Any(t => t.Key == key && t.Value == value);
    }
    public static Dictionary<string,string> TagsDictionary(this PollingStationModel pollingStation)
    {
        return pollingStation.Tags.ToDictionary(t => t.Key, t => t.Value);
    }
    

    public static List<TagModel> ToTags(this Dictionary<string,string> tags)
    {
        return tags.Select(t => new TagModel()
        {
            Key = t.Key,
            Value = t.Value
        }).ToList();
    }
}
