using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.EmergencyAggregate;

public class LocationType : SmartEnum<LocationType, string>
{
    public static readonly LocationType VisitedPollingStation = new(nameof(VisitedPollingStation), nameof(VisitedPollingStation));
    public static readonly LocationType OtherPollingStation = new(nameof(OtherPollingStation), nameof(OtherPollingStation));
    public static readonly LocationType NotRelatedToPollingStation = new(nameof(NotRelatedToPollingStation), nameof(NotRelatedToPollingStation));

    private LocationType(string name, string value) : base(name, value)
    {
    }
}
