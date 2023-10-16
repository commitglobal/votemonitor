using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Feature.PollingStation.CreatePollingStation;
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
        Put("api/polling-stations/{id:int}");

        AllowAnonymous();
    }

    public override async Task HandleAsync(PollingStationUpdateRequestDTO req, CancellationToken ct)
    {
        try
        {
            var id = Route<int>("id");

            var model = Map.ToEntity(req);

            await _repository.Update(id, model);

            await SendCreatedAtAsync<GetPollingStationEndpoint>(new { id = model.Id }, Map.FromEntity(model));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Polling Station ");

            AddError(ex.Message);
        }

        ThrowIfAnyErrors();
    }
}
