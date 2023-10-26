using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStation;
internal class GetPollingStationEndpoint : EndpointWithoutRequest<PollingStationReadDto, GetPollingStationMapper>
{
    private readonly IPollingStationRepository _repository;
    private readonly ILogger<GetPollingStationEndpoint> _logger;

    public GetPollingStationEndpoint(IPollingStationRepository repository, ILogger<GetPollingStationEndpoint> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/{id:Guid}");

        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Guid id = Route<Guid>("id");

        try
        {
            var model = await _repository.GetByIdAsync(id);

            await SendAsync(Map.FromEntity(model), cancellation: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Polling Station by ID ");

            AddError(ex.Message);
        }

        ThrowIfAnyErrors();
    }
}
