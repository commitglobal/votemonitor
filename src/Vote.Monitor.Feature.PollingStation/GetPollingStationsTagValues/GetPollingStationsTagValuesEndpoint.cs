using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Vote.Monitor.Feature.PollingStation.RequestBinders;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;
internal class GetPollingStationsTagValuesEndpoint : Endpoint<TagValuesRequest, List<TagReadDto>, GetPollingStationsTagValuesMapper>
{
    private readonly IPollingStationRepository _repository;

    public GetPollingStationsTagValuesEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/tags/values");
        RequestBinder(new TagValuesRequestBinder());

        AllowAnonymous();
    }

    public override async Task HandleAsync(TagValuesRequest req, CancellationToken ct)
    {
        var tags = await _repository.GetTagValuesAsync(req.SelectTag, req.Filter);

        await SendAsync(Map.FromEntity(tags));
    }
}
