using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;
internal class GetPollingStationsTagValuesEndpoint : Endpoint<TagValuesRequest, TagValuesResponse>
{
    private readonly IPollingStationRepository _repository;
    public GetPollingStationsTagValuesEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/tags/values");

        AllowAnonymous();
    }

    public override async Task HandleAsync(TagValuesRequest req, CancellationToken ct)
    {
        var tags = await _repository.GetTags();

        var filters = ConvertToDictionary(req);

        var query = await _repository.GetByTags(filters);
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
