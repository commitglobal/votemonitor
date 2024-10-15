namespace Job.Contracts.Jobs;

public interface IExportFormSubmissionsJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}