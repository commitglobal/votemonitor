namespace Vote.Monitor.Core.Services.Hangfire;

public interface IExportIncidentReportsJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}