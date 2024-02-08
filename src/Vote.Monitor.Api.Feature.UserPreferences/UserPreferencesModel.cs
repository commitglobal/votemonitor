namespace Vote.Monitor.Api.Feature.UserPreferences;

public class  UserPreferencesModel
{
    public Guid Id { get; init; }

    /// <summary>
    /// preferences will be a list of key, value pairs
    /// </summary>
    public Dictionary<string, string> Preferences { get; init; }

  
}
