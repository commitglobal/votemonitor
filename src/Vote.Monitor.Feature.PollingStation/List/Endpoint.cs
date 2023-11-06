namespace Vote.Monitor.Feature.PollingStation.List;
public class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<PollingStationModel>>, ProblemDetails>>
{
    private readonly IReadRepository<PollingStationAggregate> _repository;

    public Endpoint(IReadRepository<PollingStationAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/polling-stations/list");
    }

    public override async Task<Results<Ok<PagedResponse<PollingStationModel>>, ProblemDetails>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var specification = new ListPollingStationsSpecification(request.AddressFilter, request.Filter, request.PageSize, request.PageNumber);
        var pollingStations = await _repository.ListAsync(specification, ct);
        var pollingStationsCount = await _repository.CountAsync(specification, ct);
        var result = pollingStations.Select(x => new PollingStationModel
        {
            Id = x.Id,
            Address = x.Address,
            DisplayOrder = x.DisplayOrder,
            Tags = x.Tags.ToDictionary()
        }).ToList();

        return TypedResults.Ok(new PagedResponse<PollingStationModel>(result, pollingStationsCount, request.PageNumber, request.PageSize));
    }
}
