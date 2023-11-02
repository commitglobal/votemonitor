using System.Text.Json;

namespace Vote.Monitor.Domain.Entities.PollingStationAggregate;
public class PollingStation : IAggregateRoot, IDisposable
{
#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStation()
    {

    }
    
    public PollingStation(string address, int displayOrder, JsonDocument tags)
    {
        Address = address;
        DisplayOrder = displayOrder;
        Tags = tags;
    }

    public Guid Id { get; private set; }
    public string Address { get; private set; }
    public int DisplayOrder { get; private set; }
    public JsonDocument Tags { get; private set; }

    public void UpdateDetails(string address, int displayOrder, JsonDocument tags)
    {
        Address = address;
        DisplayOrder = displayOrder;
        Tags = tags;
    }

    public void Dispose()
    {
        Tags.Dispose();
    }
}
