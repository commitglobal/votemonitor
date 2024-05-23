namespace Vote.Monitor.Core.Services.Hangfire;

public interface IExportFormSubmissionsJob
{
    Task ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}
