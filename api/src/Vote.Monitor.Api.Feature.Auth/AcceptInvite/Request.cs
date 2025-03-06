namespace Vote.Monitor.Api.Feature.Auth.AcceptInvite;

public class Request
{
    public string InvitationToken { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword{ get; set; } = null!;
}
