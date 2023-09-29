using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.UpdatePollingStation;
internal class UpdatePollingStationEndpoint
{
    private readonly IPollingStationRepository _repository;

    public UpdatePollingStationEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
    }
}
