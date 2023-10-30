using System.Text.Json;

namespace Vote.Monitor.Domain.Models;
public class PollingStation : IDisposable
{
    public Guid Id { get; set; }
    public required string Address { get; set; }
    public int DisplayOrder { get; set; }
    public JsonDocument Tags { get; set; }

    public void Dispose()
    {
        Tags.Dispose();
    }
}
