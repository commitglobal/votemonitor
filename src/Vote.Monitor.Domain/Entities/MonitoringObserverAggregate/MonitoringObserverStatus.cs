using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

public sealed class MonitoringObserverStatus : SmartEnum<MonitoringObserverStatus, string>
{
    public static readonly MonitoringObserverStatus Active = new(nameof(Active), nameof(Active));
    public static readonly MonitoringObserverStatus Suspended = new(nameof(Suspended), nameof(Suspended));

    private MonitoringObserverStatus(string name, string value) : base(name, value)
    {
    }
}
