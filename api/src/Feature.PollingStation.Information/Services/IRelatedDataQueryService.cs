namespace Feature.PollingStation.Information.Services;

public interface IRelatedDataQueryService
{
    Task<bool> GetHasDataForCurrentPollingStationAsync(Guid electionRoundId,
        Guid pollingStationId,
        Guid observerId,
        CancellationToken cancellationToken = default);
}
