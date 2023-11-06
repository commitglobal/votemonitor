using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.CSOAggregate;

public sealed class CSOStatus : SmartEnum<CSOStatus, string>
{
    public static readonly CSOStatus Activated = new(nameof(Activated), nameof(Activated));
    public static readonly CSOStatus Deactivated = new(nameof(Deactivated), nameof(Deactivated));

    private CSOStatus(string name, string value) : base(name, value)
    {
    }
}
