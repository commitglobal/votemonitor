using System.Text.Json;

namespace Vote.Monitor.Domain.Models;

public static class TagModelExtensions
{
    public static JsonDocument ToTags(this Dictionary<string, string> tags)
    {
        return JsonSerializer.SerializeToDocument(tags);
    }
    
    public static Dictionary<string, string>? ToDictionary(this JsonDocument document)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>?>(document);
    }
}
