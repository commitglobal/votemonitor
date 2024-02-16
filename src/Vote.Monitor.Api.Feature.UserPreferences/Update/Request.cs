using Microsoft.IdentityModel.JsonWebTokens;

namespace Vote.Monitor.Api.Feature.UserPreferences.Update;
public class Request
{
    [FromClaim(JwtRegisteredClaimNames.Sub)]
    public Guid Id { get; set; }
    public Guid LanguageId { get; set; }
}
