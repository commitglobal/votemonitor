namespace Vote.Monitor.Observer.Create;

public class Request
{
    public Guid CSOId { get; set; }
    public string Name { get;  set; }
    public string Login { get;  set; }
    public string Password { get;  set; }
}
