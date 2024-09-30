namespace Vote.Monitor.Core.Services.Hangfire;

public interface IExportIssueReportsJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}