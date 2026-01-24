namespace Vote.Monitor.Domain.Entities.PollingStationAggregate;

public class PollingStation : AuditableBaseEntity, IAggregateRoot, IDisposable
{
#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStation()
    {
    }
#pragma warning restore CS8618
    
    public static PollingStation Create(
        Guid? id,
        ElectionRound electionRound,
        string level1,
        string level2,
        string level3,
        string level4,
        string level5,
        string number,
        string address,
        int displayOrder,
        JsonDocument tags,
        double? latitude,
        double? longitude,
        DateTime createdOn,
        Guid userId)
    {
        var pollingStation = new PollingStation(id, electionRound, level1, level2, level3, level4, level5, number, address,
            displayOrder,
            tags, latitude, longitude);

        pollingStation.CreatedOn = createdOn;
        pollingStation.CreatedBy = userId;

        return pollingStation;
    }
    
    public static PollingStation Create(
        ElectionRound electionRound,
        string level1,
        string level2,
        string level3,
        string level4,
        string level5,
        string number,
        string address,
        int displayOrder,
        JsonDocument tags,
        double? latitude,
        double? longitude,
        DateTime createdOn,
        Guid userId)
    {
        return Create(null, electionRound, level1, level2, level3, level4, level5, number, address,displayOrder, tags, latitude, longitude, createdOn, userId);
    }

    public Guid Id { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid ElectionRoundId { get; private set; }

    internal PollingStation(
        Guid? id,
        ElectionRound electionRound,
        string level1,
        string level2,
        string level3,
        string level4,
        string level5,
        string number,
        string address,
        int displayOrder,
        JsonDocument tags,
        double? latitude,
        double? longitude)
    {
        Id = id ?? Guid.NewGuid();
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
        Latitude = latitude;
        Longitude = longitude;
    }

    public string Level1 { get; private set; }
    public string Level2 { get; private set; }
    public string Level3 { get; private set; }
    public string Level4 { get; private set; }
    public string Level5 { get; private set; }
    public string Number { get; private set; }

    public string Address { get; private set; }
    public int DisplayOrder { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    public JsonDocument Tags { get; private set; }

    public void UpdateDetails(string level1,
        string level2,
        string level3,
        string level4,
        string level5,
        string number,
        string address,
        int displayOrder,
        JsonDocument tags,
        double? latitude,
        double? longitude)
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
        Latitude = latitude;
        Longitude = longitude;
    }

    public void Dispose()
    {
        Tags.Dispose();
    }
}
