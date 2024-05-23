namespace Vote.Monitor.Core.Services.Hangfire;

public interface IExportQuickReportsJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}
