namespace Vote.Monitor.Api.Feature.NgoAdmin.Update;

public class Request
{
    public Guid NgoId { get; set; }
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
