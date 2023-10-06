using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStation;
internal class GetPollingStationEndpoint : EndpointWithoutRequest<PollingStationReadDto, GetPollingStationMapper>
{
    private readonly IPollingStationRepository _repository;

    public GetPollingStationEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/{id:int}");

        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        int id = Route<int>("id");

        var model = _repository.GetById(id);
        
        await SendAsync(Map.FromEntity(model.Result));
    }
}
