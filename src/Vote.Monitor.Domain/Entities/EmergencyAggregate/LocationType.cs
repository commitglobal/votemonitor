using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.EmergencyAggregate;

public class LocationType : SmartEnum<LocationType, string>
{
    public static readonly LocationType VisitedPollingStation = new(nameof(VisitedPollingStation), nameof(VisitedPollingStation));
    public static readonly LocationType OtherPollingStation = new(nameof(OtherPollingStation), nameof(OtherPollingStation));
    public static readonly LocationType NotRelatedToPollingStation = new(nameof(NotRelatedToPollingStation), nameof(NotRelatedToPollingStation));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="LocationType" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out LocationType result)
    {
        return TryFromValue(value, out result);
    }

    private LocationType(string name, string value) : base(name, value)
    {
    }
}
