
using FastEndpoints;
using Microsoft.Extensions.Logging;
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
        var tags = await _repository.GetAllTagKeysAsync();
        //get distinct keys from tags


        await SendAsync(tags);
    }
}
