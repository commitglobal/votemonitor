namespace Vote.Monitor.Api.Feature.Auth.Login;

public class Request
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
