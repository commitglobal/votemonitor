namespace Feature.Form.Submissions.Services;

public interface IOrphanedDataCleanerService
{
    Task CleanupAsync(Guid electionRoundId,
        Guid monitoringObserverId,
        Guid pollingStationId,
        Guid formId,
        Guid[] questionIds,
        CancellationToken cancellationToken = default);

    Task CleanupAsync(Guid electionRoundId,
        Guid monitoringObserverId,
        Guid pollingStationId,
        Guid formId,
        CancellationToken cancellationToken = default);
}
