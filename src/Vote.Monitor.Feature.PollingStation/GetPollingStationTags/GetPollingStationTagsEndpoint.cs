
using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationTags;
internal class GetPollingStationTagsEndpoint : EndpointWithoutRequest
{
    private readonly IPollingStationRepository _repository;
    public GetPollingStationTagsEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/tags");

        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
       var tags = await _repository.GetTags();

       await SendAsync(tags, cancellation: ct);
    }
}
