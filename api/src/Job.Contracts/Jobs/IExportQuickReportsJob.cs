namespace Job.Contracts.Jobs;

public interface IExportQuickReportsJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}