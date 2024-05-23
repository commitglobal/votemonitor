namespace Vote.Monitor.Core.Services.Hangfire;

public interface IExportFormSubmissionsJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}
