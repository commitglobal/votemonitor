using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.CSOAggregate;

public sealed class CSOStatus : SmartEnum<CSOStatus>
{
    public static readonly CSOStatus Activated = new(nameof(Activated), 1);
    public static readonly CSOStatus Deactivated = new(nameof(Deactivated), 2);

    private CSOStatus(string name, int value) : base(name, value)
    {
    }


    public static bool TryParse(string s, out CSOStatus result)
    {
        return CSOStatus.TryFromName(s, out result);
    }
}
