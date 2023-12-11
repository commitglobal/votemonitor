using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;
public sealed class ImportType : SmartEnum<ImportType, string>
{
    public static readonly ImportType PollingStation = new(nameof(PollingStation), nameof(PollingStation));
    public static readonly ImportType Observer = new(nameof(Observer), nameof(Observer));
    private ImportType(string name, string value) : base(name, value)
    {
    }
}


