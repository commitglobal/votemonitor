using System.Text.Json;
using Vote.Monitor.Core.Services.Time;

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
        ITimeService timeService) : base(Guid.NewGuid(), timeService)
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
