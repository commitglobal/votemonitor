using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Feature.PollingStation.CreatePollingStation;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.DeletePollingStation;
internal class DeletePollingStationEndpoint : EndpointWithoutRequest<PollingStationReadDto>
{
    private readonly IPollingStationRepository _repository;
    private readonly ILogger<DeletePollingStationEndpoint> _logger;

    public DeletePollingStationEndpoint(IPollingStationRepository repository, ILogger<DeletePollingStationEndpoint> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/api/polling-stations/{id:int}");

        AllowAnonymous();
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            int id = Route<int>("id");

        await _repository.DeleteAsync(id);

            await SendNoContentAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while removing Polling Station ");

            AddError(ex.Message);
        }

        ThrowIfAnyErrors();
    }
}
