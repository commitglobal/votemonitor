namespace Vote.Monitor.ElectionRound.Create;

public class Request
{
    public Guid CSOId { get; set; }
    public required string Name { get;  set; }
    public required string Login { get;  set; }
    public required string Password { get;  set; }
}
