namespace Vote.Monitor.Api.Feature.UserPreferences.Update;
public class Request
{
    [FromClaim("Sub")]
    public Guid Id { get; set; }
    public string? LanguageIso { get; set; }
    public Dictionary<string, string>? Preferences { get; set; }
    public Guid? LanguageId { get; set; }
}
