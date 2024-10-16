namespace Job.Contracts.Jobs;

public interface IExportCitizenReportsJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}