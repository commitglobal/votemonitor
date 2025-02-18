namespace Vote.Monitor.Api.Feature.NgoAdmin.Create;

public class Request
{
    public Guid NgoId { get; set; }
    public string FirstName { get;  set; }
    public string LastName { get;  set; }
    public string Email { get;  set; }
    public string? PhoneNumber { get;  set; }
    public string Password { get;  set; }
}
