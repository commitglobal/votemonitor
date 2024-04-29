namespace Vote.Monitor.Hangfire.Jobs.ExportData;

public class ExportedDataWasNotFoundException : Exception
{
    public ExportedDataWasNotFoundException()
    {
    }

    public ExportedDataWasNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public ExportedDataWasNotFoundException(Guid electionRoundId, Guid ngoId, Guid exportedDataId) : base($"ExportData was not found for {electionRoundId} {ngoId} {exportedDataId}")
    {
    }
}
