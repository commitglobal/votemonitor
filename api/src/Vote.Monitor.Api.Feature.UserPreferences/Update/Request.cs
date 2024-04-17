using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.UserPreferences.Update;
public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid Id { get; set; }
    public string LanguageCode { get; set; }
}
