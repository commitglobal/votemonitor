using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public sealed class FormTemplateStatus : SmartEnum<FormTemplateStatus, string>
{
    public static readonly FormTemplateStatus Drafted = new(nameof(Drafted), nameof(Drafted));
    public static readonly FormTemplateStatus Published = new(nameof(Published), nameof(Published));

    private FormTemplateStatus(string name, string value) : base(name, value)
    {
    }
}
