using Vote.Monitor.Core.Entities;

namespace Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;
public class ImportValidationErrors : BaseEntity, IAggregateRoot
{

#pragma warning disable CS8618 // Required by Entity Framework
    private ImportValidationErrors()
    {

    }

    public ImportValidationErrors(ImportType importType, string originalFileName, string data, DateTime date)
    {
        ImportType = importType;
        OriginalFileName = originalFileName;
        Data = data;
        Date = date.ToUniversalTime();
    }

    public ImportType ImportType { get; private set; }

    public string OriginalFileName { get; private set; }
    public DateTime Date { get; private set; }

    public string Data { get; private set; }


}
