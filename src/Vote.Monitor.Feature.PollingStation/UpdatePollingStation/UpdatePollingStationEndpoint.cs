using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.UpdatePollingStation;
internal class UpdatePollingStationEndpoint : Endpoint<PollingStationUpdateRequestDTO, PollingStationReadDto, UpdatePollingStationMapper>
{
    private readonly IPollingStationRepository _repository;

    public UpdatePollingStationEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("api/polling-stations/{id:int}");

        AllowAnonymous();
    }

    public override async Task HandleAsync(PollingStationUpdateRequestDTO req, CancellationToken ct)
    {
        var id = Route<int>("id");

        var model = Map.ToEntity(req);

        await _repository.UpdateAsync(id, model);

        await SendCreatedAtAsync<GetPollingStationEndpoint>(new { id = model.Id }, Map.FromEntity(model));
    }
}
