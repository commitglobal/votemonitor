namespace Vote.Monitor.Core.Services.Hangfire;

public interface IExportPollingStationsJob
{
    Task Run(Guid electionRoundId, Guid exportedDataId, CancellationToken ct);
}
