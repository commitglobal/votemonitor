namespace Vote.Monitor.Api.Feature.Observer.Update;

public class Request
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
}
