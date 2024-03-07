using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public sealed class FormType : SmartEnum<FormType, string>
{
    public static readonly FormType Opening = new(nameof(Opening), nameof(Opening));
    public static readonly FormType Voting = new(nameof(Voting), nameof(Voting));
    public static readonly FormType ClosingAndCounting = new(nameof(ClosingAndCounting), nameof(ClosingAndCounting));

    private FormType(string name, string value) : base(name, value)
    {
    }
}
