using System.Security.Claims;
using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Extensions;

public class CustomSentryUserFactory : ISentryUserFactory
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public CustomSentryUserFactory()
    {
    }

    public CustomSentryUserFactory(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public SentryUser? Create() => _httpContextAccessor?.HttpContext is { } httpContext ? Create(httpContext) : null;

    public SentryUser? Create(HttpContext context)
    {
        var principal = context.User;
        if (principal is null)
        {
            return null;
        }

        string? identifier = null;
        string? sub = null;
        string? ngoId = null;
        string? role = null;
        string? email = null;
        string? username = null;

        foreach (var claim in principal.Claims)
        {
            switch (claim.Type)
            {
                case ApplicationClaimTypes.UserId:
                    sub = claim.Value;
                    break;
                case ApplicationClaimTypes.NgoId:
                    ngoId = claim.Value;
                    break;
                case ApplicationClaimTypes.Role:
                    role = claim.Value;
                    break;
                case ClaimTypes.Email:
                    email = claim.Value;
                    break;
                case ClaimTypes.NameIdentifier:
                    identifier = claim.Value;
                    break;
                case ClaimTypes.Name:
                    username = claim.Value;
                    break;
            }
        }

        // Identity.Name Reads the value of: ClaimsIdentity.NameClaimType which by default is ClaimTypes.Name
        // It can be changed by the application to read a different claim though:
        var name = principal.Identity?.Name;
        if (name is not null && username != name)
        {
            username = name;
        }

        var ipAddress = context.Connection?.RemoteIpAddress?.ToString();
        var extraData = new Dictionary<string, string>();

        if (role is not null)
        {
            extraData.Add(ApplicationClaimTypes.Role, role);
        }

        if (ngoId is not null)
        {
            extraData.Add(ApplicationClaimTypes.NgoId, ngoId);
        }

        if (sub is not null)
        {
            extraData.Add(ApplicationClaimTypes.UserId, sub);
        }

        return email is null && identifier is null && username is null && ipAddress is null && role is null && ngoId is null && sub is null
            ? null
            : new SentryUser
            {
                Id = identifier,
                Other = extraData,
                Email = email,
                Username = username,
                IpAddress = ipAddress,
            };
    }

}
