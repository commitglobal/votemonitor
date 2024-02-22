namespace Vote.Monitor.Api.Feature.UserPreferences.Get;

public class Request
{
    [FromClaim("Sub")]
    public Guid Id { get; set; }
}
