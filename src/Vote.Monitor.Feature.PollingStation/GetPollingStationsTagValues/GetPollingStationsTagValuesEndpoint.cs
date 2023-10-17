using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.CreatePollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;
internal class GetPollingStationsTagValuesEndpoint : Endpoint<TagValuesRequest, TagValuesResponse>
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

        var filters = ConvertToDictionary(req);

        IEnumerable<TagModel> tags = await _repository.GetAllAsync(filters,0, 0);
        //TO DO : implement 
        //var query = await _repository.GetByTags(filters);
    }

    private Dictionary<string, string> ConvertToDictionary(TagValuesRequest request)
    {
        var dictionary = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(request.SelectTag))
        {
            dictionary["selectTag"] = request.SelectTag;
        }

        if (!string.IsNullOrEmpty(request.Filter))
        {
            dictionary["filter"] = request.Filter;
        }

        return dictionary;
    }
}
