using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public sealed class FormTemplateType : SmartEnum<FormTemplateType, string>
{
    public static readonly FormTemplateType Opening = new(nameof(Opening), nameof(Opening));
    public static readonly FormTemplateType Voting = new(nameof(Voting), nameof(Voting));
    public static readonly FormTemplateType ClosingAndCounting = new(nameof(ClosingAndCounting), nameof(ClosingAndCounting));

    private FormTemplateType(string name, string value) : base(name, value)
    {
    }
}
