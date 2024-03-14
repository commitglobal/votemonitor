using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormAggregate;

public sealed class FormStatus : SmartEnum<FormStatus, string>
{
    public static readonly FormStatus Drafted = new(nameof(Drafted), nameof(Drafted));
    public static readonly FormStatus Published = new(nameof(Published), nameof(Published));
    public static readonly FormStatus Obsolete = new(nameof(Obsolete), nameof(Obsolete));

    private FormStatus(string name, string value) : base(name, value)
    {
    }
}
