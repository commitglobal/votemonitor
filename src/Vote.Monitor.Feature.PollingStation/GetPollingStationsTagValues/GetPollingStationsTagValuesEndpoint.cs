using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;
internal class GetPollingStationsTagValuesEndpoint : Endpoint<TagValuesRequest, List<TagReadDto>, GetPollingStationsTagValuesMapper>
{
    private readonly ITagRepository _repository;
    private readonly ILogger<GetPollingStationsTagValuesEndpoint> _logger;

    public GetPollingStationsTagValuesEndpoint(ITagRepository repository, ILogger<GetPollingStationsTagValuesEndpoint> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/tags/values");

        AllowAnonymous();
    }

    public override async Task HandleAsync(TagValuesRequest req, CancellationToken ct)
    {
        List<TagModel>? filters = null;
        if (req.Filter != null) filters = TagModelExtensions.DecodeFilter(req.Filter);

        List<TagModel> tags = (await _repository.GetTagsAsync(req.SelectTag, filters)).ToList();

        await SendAsync(Map.FromEntity(tags));
    }


}
