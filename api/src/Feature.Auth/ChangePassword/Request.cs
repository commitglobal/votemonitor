using Vote.Monitor.Core.Security;

namespace Feature.Auth.ChangePassword;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public string UserId { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmNewPassword { get; set; } = null!;
}
