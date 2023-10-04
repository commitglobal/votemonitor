using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.DeletePollingStation;
internal class DeletePollingStationEndpoint: EndpointWithoutRequest<PollingStationReadDto>
{
    private readonly IPollingStationRepository _repository;
    public DeletePollingStationEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/api/polling-stations/{id:int}");

        AllowAnonymous();
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        int id = Route<int>("id");

        await _repository.Delete(id);

        await SendNoContentAsync();
    }
}
