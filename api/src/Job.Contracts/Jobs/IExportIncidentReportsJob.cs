namespace Job.Contracts.Jobs;

public interface IExportIncidentReportsJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}