namespace Job.Contracts.Jobs;

public interface IExportFormSubmissionsSimplifiedJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}
