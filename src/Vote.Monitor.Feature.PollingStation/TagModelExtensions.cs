using System.Text.Json;

namespace Vote.Monitor.Feature.PollingStation;

public static class TagModelExtensions
{
    public static JsonDocument ToTagsObject(this Dictionary<string, string> tags)
    {
        return JsonSerializer.SerializeToDocument(tags);
    }
    
    public static Dictionary<string, string> ToDictionary(this JsonDocument document)
    {
        return document.Deserialize<Dictionary<string, string>?>()!;
    }
}
