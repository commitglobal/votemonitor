using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.UpdatePollingStation;
internal class UpdatePollingStationEndpoint : Endpoint<PollingStationUpdateRequestDTO, PollingStationReadDto, UpdatePollingStationMapper>
{
    private readonly IPollingStationRepository _repository;
    private readonly ILogger<UpdatePollingStationEndpoint> _logger;

    public UpdatePollingStationEndpoint(IPollingStationRepository repository, ILogger<UpdatePollingStationEndpoint> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("api/polling-stations/{id:Guid}");

        AllowAnonymous();
    }

    public override async Task HandleAsync(PollingStationUpdateRequestDTO req, CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var model = Map.ToEntity(req);

        await _repository.UpdateAsync(id, model);

        await SendCreatedAtAsync<GetPollingStationEndpoint>(new { id = model.Id }, Map.FromEntity(model));
    }
}
