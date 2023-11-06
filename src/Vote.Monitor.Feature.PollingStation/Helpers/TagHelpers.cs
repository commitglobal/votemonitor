namespace Vote.Monitor.Feature.PollingStation.Helpers;

public static class TagHelpers
{
    public static JsonDocument ToTagsObject(this Dictionary<string, string> tags)
    {
        return JsonSerializer.SerializeToDocument(tags);
    }

    public static JsonDocument ToTagsObject(this IEnumerable<TagImportModel> tags)
    {
        var tagsDictionary = tags.ToDictionary(k => k.Name!, v => v.Value!);
        return JsonSerializer.SerializeToDocument(tagsDictionary);
    }

    public static Dictionary<string, string> ToDictionary(this JsonDocument document)
    {
        return document.Deserialize<Dictionary<string, string>>()!;
    }
}
