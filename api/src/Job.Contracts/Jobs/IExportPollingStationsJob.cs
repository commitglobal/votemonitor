namespace Job.Contracts.Jobs;

public interface IExportPollingStationsJob
{
    Task Run(Guid electionRoundId, Guid exportedDataId, CancellationToken ct);
}