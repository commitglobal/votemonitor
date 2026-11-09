namespace Job.Contracts.Jobs;

public interface IExportIncidentReportsSimplifiedJob
{
    Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct);
}
