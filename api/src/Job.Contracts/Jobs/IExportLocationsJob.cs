namespace Job.Contracts.Jobs;

public interface IExportLocationsJob
{
    Task Run(Guid electionRoundId, Guid exportedDataId, CancellationToken ct);
}