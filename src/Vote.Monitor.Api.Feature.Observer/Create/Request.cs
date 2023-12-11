namespace Vote.Monitor.Api.Feature.Observer.Create;

public class Request
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PhoneNumber { get; set; }
}
