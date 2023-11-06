using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

public sealed class ElectionRoundStatus : SmartEnum<ElectionRoundStatus, string>
{
    public static readonly ElectionRoundStatus NotStarted = new(nameof(NotStarted), nameof(NotStarted));
    public static readonly ElectionRoundStatus Started = new(nameof(Started), nameof(Started));
    public static readonly ElectionRoundStatus Archived = new(nameof(Archived), nameof(Archived));

    private ElectionRoundStatus(string name, string value) : base(name, value)
    {
    }
}
