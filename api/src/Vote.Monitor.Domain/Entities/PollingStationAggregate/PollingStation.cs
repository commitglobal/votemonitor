namespace Vote.Monitor.Domain.Entities.PollingStationAggregate;

public class PollingStation : AuditableBaseEntity, IAggregateRoot, IDisposable
{
#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStation()
    {
    }
#pragma warning restore CS8618

    internal PollingStation(ElectionRound electionRound,
        string level1,
        string level2,
        string level3,
        string level4,
        string level5,
        string number,
        string address,
        int displayOrder,
        JsonDocument tags) : this(Guid.NewGuid(), electionRound, level1, level2, level3, level4, level5, number, address, displayOrder, tags)
    {
    }

    public static PollingStation Create(ElectionRound electionRound,
        string level1,
        string level2,
        string level3,
        string level4,
        string level5,
        string number,
        string address,
        int displayOrder,
        JsonDocument tags,
        DateTime createdOn,
        Guid userId)
    {
        var pollingStation = new PollingStation(electionRound, level1, level2, level3, level4, level5, number, address, displayOrder,
            tags);

        pollingStation.CreatedOn = createdOn;
        pollingStation.CreatedBy = userId;

        return pollingStation;
    }

    public Guid Id { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid ElectionRoundId { get; private set; }

    internal PollingStation(
        Guid id,
        ElectionRound electionRound,
        string level1,
        string level2,
        string level3,
        string level4,
        string level5,
        string number,
        string address,
        int displayOrder,
        JsonDocument tags)
    {
        Id = id;
        ElectionRoundId = electionRound.Id;
        ElectionRound = electionRound;
        Level1 = level1;
        Level2 = level2;
        Level3 = level3;
        Level4 = level4;
        Level5 = level5;
        Number = number;
        Address = address;
        DisplayOrder = displayOrder;
        Tags = tags;
    }

    public string Level1 { get; private set; }
    public string Level2 { get; private set; }
    public string Level3 { get; private set; }
    public string Level4 { get; private set; }
    public string Level5 { get; private set; }
    public string Number { get; private set; }

    public string Address { get; private set; }
    public int DisplayOrder { get; private set; }

    public JsonDocument Tags { get; private set; }

    public void UpdateDetails(string level1,
        string level2,
        string level3,
        string level4,
        string level5,
        string number,
        string address,
        int displayOrder,
        JsonDocument tags)
    {
        Level1 = level1;
        Level2 = level2;
        Level3 = level3;
        Level4 = level4;
        Level5 = level5;
        Number = number;
        Address = address;
        DisplayOrder = displayOrder;
        Tags = tags;
    }
    public void Dispose()
    {
        Tags.Dispose();
    }
}
