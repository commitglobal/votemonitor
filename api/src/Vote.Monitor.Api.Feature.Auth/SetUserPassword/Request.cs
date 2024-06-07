namespace Vote.Monitor.Api.Feature.Auth.SetUserPassword;

public class Request
{
    public Guid AspNetUserId { get; set; }
    public string NewPassword{ get; set; }
}
