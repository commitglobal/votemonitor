namespace Feature.NgoCoalitions.Services;

public interface IFormSubmissionsCleanupService
{
    /// <summary>
    /// Removes all data submitted by a ngo ,ex member of the coalition
    /// </summary>
    /// <param name="electionRoundId"></param>
    /// <param name="coalitionId"></param>
    /// <param name="monitoringNgoId"></param>
    /// <returns></returns>
    Task CleanupFormSubmissionsAsync(Guid electionRoundId, Guid coalitionId, Guid monitoringNgoId);

    /// <summary>
    /// Removes all data submitted by a ngo for a form when form access is revoked
    /// </summary>
    /// <param name="electionRoundId"></param>
    /// <param name="coalitionId"></param>
    /// <param name="monitoringNgoId"></param>
    /// <param name="formId"></param>
    /// <returns></returns>
    Task CleanupFormSubmissionsAsync(Guid electionRoundId, Guid coalitionId, Guid monitoringNgoId, Guid formId);
}
