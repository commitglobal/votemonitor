using System.Text.Json;

namespace Vote.Monitor.Api.Feature.UserPreferences.Helpers;

public static class Extensions
{
    public static JsonDocument toPreferencesObject(this Dictionary<string, string> preferences)
    {
        var json = JsonSerializer.SerializeToDocument(preferences);
        return json;
    }

    public static Dictionary<string, string> toPreferencesObject(this JsonDocument? preferences)
    {
        if (preferences == null)
        {
            return [];
        }
        var dic = JsonSerializer.Deserialize<Dictionary<string, string>>(preferences);

        return dic ?? new();
    }

}
