using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Auth.AcceptInvite;

public class Request
{
    public string InviteCode { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmNewPassword { get; set; } = default!;
}
