namespace Vote.Monitor.Feature.PollingStation;

public static class TagModelExtensions
{
    public static JsonDocument ToTagsObject(this Dictionary<string, string> tags)
    {
        return JsonSerializer.SerializeToDocument(tags);
    }
    
    public static JsonDocument ToTagsObject(this List<TagImportModel> tags)
    {
        var tagsDictionary = tags.ToDictionary(k=> k.Name!, v=>v.Value!);
        return JsonSerializer.SerializeToDocument(tagsDictionary);
    }
    
    public static Dictionary<string, string> ToDictionary(this JsonDocument document)
    {
        return document.Deserialize<Dictionary<string, string>?>()!;
    }
}
