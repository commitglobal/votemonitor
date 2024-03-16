namespace Vote.Monitor.Domain.Entities.PollingStationAggregate;

public class PollingStation : AuditableBaseEntity, IAggregateRoot, IDisposable
{
#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStation()
    {
    }
#pragma warning restore CS8618

    public PollingStation(string address,
        int displayOrder,
        JsonDocument tags,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        Address = address;
        DisplayOrder = displayOrder;
        Tags = tags;
    }

    internal PollingStation(Guid id, 
        string address, 
        int displayOrder,
        JsonDocument tags, 
        ITimeProvider timeProvider) : base(id, timeProvider)
    { 
        Address = address;
        DisplayOrder = displayOrder;
        Tags = tags;
    }

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
