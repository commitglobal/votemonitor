namespace Vote.Monitor.Domain.Models;

public static class TagModelExtensions
{
    public static bool HasTag(this PollingStationModel pollingStation, string key, string value)
    {
        return pollingStation.Tags.Any(t => t.Key == key && t.Value == value);
    }
    public static Dictionary<string, string> TagsDictionary(this PollingStationModel pollingStation)
    {
        return pollingStation.Tags.ToDictionary(t => t.Key, t => t.Value);
    }


    public static List<TagModel> ToTags(this Dictionary<string, string> tags)
    {
        return tags.Select(t => new TagModel()
        {
            Key = t.Key,
            Value = t.Value
        }).ToList();
    }


    public static List<TagModel> DecodeFilter(string filterString, char separator = ',')
    {
        var filterDict = new List<TagModel>();

        foreach (var filterPair in filterString.Split(separator))
        {
            var keyValue = filterPair.Split(':');
            filterDict.Add(new TagModel { Key = keyValue[0], Value = keyValue[1] });
        }

        return filterDict;
    }


}
