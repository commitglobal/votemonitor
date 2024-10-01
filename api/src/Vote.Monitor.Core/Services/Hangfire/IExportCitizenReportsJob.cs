namespace Vote.Monitor.Core.Services.Hangfire;

public interface IExportCitizenReportsJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}