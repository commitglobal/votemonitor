namespace Job.Contracts.Jobs;

public interface IExportPollingStationInformationJob
{
    Task Run(Guid electionRoundId, Guid exportedDataId, CancellationToken ct);
}
