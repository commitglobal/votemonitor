namespace Vote.Monitor.Domain.Entities.PollingStationAggregate;

public class PollingStation : AuditableBaseEntity, IAggregateRoot, IDisposable
{
#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStation()
    {
    }
#pragma warning restore CS8618

    public PollingStation(ElectionRound electionRound, string address,
        int displayOrder,
        JsonDocument tags,
        ITimeProvider timeProvider) : this(Guid.NewGuid(), electionRound, address, displayOrder, tags, timeProvider)
    {
    }

    public ElectionRound ElectionRound { get; private set; }
    public Guid ElectionRoundId { get; private set; }

    internal PollingStation(
        Guid id, 
        ElectionRound electionRound,
        string address, 
        int displayOrder,
        JsonDocument tags, 
        ITimeProvider timeProvider) : base(id, timeProvider)
    { 
        ElectionRoundId = electionRound.Id;
        ElectionRound = electionRound;
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
