namespace Vote.Monitor.Api.Feature.Observer.Update;

public class Request
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
}
