namespace Vote.Monitor.Api.Feature.Auth.AcceptInvite;

public class Request
{
    public string InvitationToken { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword{ get; set; } = default!;
}
