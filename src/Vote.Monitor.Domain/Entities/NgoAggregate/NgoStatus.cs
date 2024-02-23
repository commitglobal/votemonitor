using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.NgoAggregate;

public sealed class NgoStatus : SmartEnum<NgoStatus, string>
{
    public static readonly NgoStatus Activated = new(nameof(Activated), nameof(Activated));
    public static readonly NgoStatus Deactivated = new(nameof(Deactivated), nameof(Deactivated));

    private NgoStatus(string name, string value) : base(name, value)
    {
    }
}
