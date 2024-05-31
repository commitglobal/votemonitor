using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Vote.Monitor.Hangfire.Jobs.Export;

public class ExportedDataWasNotFoundException : Exception
{
    public ExportedDataWasNotFoundException()
    {
    }

    public ExportedDataWasNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public ExportedDataWasNotFoundException(ExportedDataType exportedDataType, Guid electionRoundId, Guid exportedDataId) : base($"ExportData was not found for {exportedDataType.Name} {electionRoundId} {exportedDataId}")
    {
    }
}
