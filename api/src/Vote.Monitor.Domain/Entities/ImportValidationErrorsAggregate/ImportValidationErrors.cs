namespace Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;
public class ImportValidationErrors : AuditableBaseEntity, IAggregateRoot
{

#pragma warning disable CS8618 // Required by Entity Framework
    private ImportValidationErrors()
    {

    }
#pragma warning restore CS8618

    public ImportValidationErrors(ImportType importType, string originalFileName, string data)
    {
        Id = Guid.NewGuid();
        ImportType = importType;
        OriginalFileName = originalFileName;
        Data = data;
    }

    public Guid Id { get; private set; }
    public ImportType ImportType { get; private set; }
    public string OriginalFileName { get; private set; }
    public string Data { get; private set; }
}
