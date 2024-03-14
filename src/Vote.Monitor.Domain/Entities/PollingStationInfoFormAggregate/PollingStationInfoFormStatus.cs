using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

public sealed class PollingStationInfoFormStatus : SmartEnum<PollingStationInfoFormStatus, string>
{
    public static readonly PollingStationInfoFormStatus Drafted = new(nameof(Drafted), nameof(Drafted));
    public static readonly PollingStationInfoFormStatus Published = new(nameof(Published), nameof(Published));

    private PollingStationInfoFormStatus(string name, string value) : base(name, value)
    {
    }
}
