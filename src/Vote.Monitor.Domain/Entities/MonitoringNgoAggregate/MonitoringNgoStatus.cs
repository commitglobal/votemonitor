using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

public sealed class MonitoringNgoStatus : SmartEnum<MonitoringNgoStatus, string>
{
    public static readonly MonitoringNgoStatus Active = new(nameof(Active), nameof(Active));
    public static readonly MonitoringNgoStatus Suspended = new(nameof(Suspended), nameof(Suspended));

    private MonitoringNgoStatus(string name, string value) : base(name, value)
    {
    }
}
