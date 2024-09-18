namespace Vote.Monitor.Core.Services.Hangfire;

public interface IExportLocationsJob
{
    Task Run(Guid electionRoundId, Guid exportedDataId, CancellationToken ct);
}