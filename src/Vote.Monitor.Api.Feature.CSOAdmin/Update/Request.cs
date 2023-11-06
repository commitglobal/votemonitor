namespace Vote.Monitor.Api.Feature.CSOAdmin.Update;

public class Request
{
    public Guid CSOId { get; set; }
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
