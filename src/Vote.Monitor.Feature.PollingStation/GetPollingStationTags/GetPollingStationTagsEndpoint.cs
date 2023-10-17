
using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Feature.PollingStation.CreatePollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationTags;
internal class GetPollingStationTagsEndpoint : EndpointWithoutRequest
{
    private readonly ITagRepository _repository;
    private readonly ILogger<GetPollingStationTagsEndpoint> _logger;

    public GetPollingStationTagsEndpoint(ITagRepository repository, ILogger<GetPollingStationTagsEndpoint> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/tags");

        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var tags = await _repository.GetAllAsync(0,1);

            await SendAsync(tags, cancellation: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve Polling Stations Tags ");

            AddError(ex.Message);
        }

        ThrowIfAnyErrors();
    }
}
