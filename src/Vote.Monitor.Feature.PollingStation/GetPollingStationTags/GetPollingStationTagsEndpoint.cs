
using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationTags;
internal class GetPollingStationTagsEndpoint : EndpointWithoutRequest
{
    private readonly IPollingStationRepository _repository;
    private readonly ILogger<GetPollingStationTagsEndpoint> _logger;

    public GetPollingStationTagsEndpoint(IPollingStationRepository repository, ILogger<GetPollingStationTagsEndpoint> logger)
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
            var tags = await _repository.GetTagKeys(null);
            //get distinct keys from tags


            await SendAsync(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve Polling Stations Tags ");

            AddError(ex.Message);
        }

        ThrowIfAnyErrors();
    }
}
