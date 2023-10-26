using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

public sealed class ElectionRoundStatus : SmartEnum<ElectionRoundStatus>
{
    public static readonly ElectionRoundStatus NotStarted = new(nameof(NotStarted), 1);
    public static readonly ElectionRoundStatus Started = new(nameof(Started), 2);
    public static readonly ElectionRoundStatus Archived = new(nameof(Archived), 3);

    private ElectionRoundStatus(string name, int value) : base(name, value)
    {
    }
}
