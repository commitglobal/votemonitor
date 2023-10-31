namespace Vote.Monitor.CSOAdmin.Update;

public class Request
{
    public Guid CSOId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
