namespace Vote.Monitor.Api.Feature.NgoAdmin.Create;

public class Request
{
    public Guid NgoId { get; set; }
    public required string Name { get;  set; }
    public required string Login { get;  set; }
    public required string Password { get;  set; }
}
