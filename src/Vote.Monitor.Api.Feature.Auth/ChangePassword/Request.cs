using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Auth.ChangePassword;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public string UserId { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmNewPassword { get; set; } = default!;
}
