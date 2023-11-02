using Vote.Monitor.Core.Models;

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
        Get("/api/polling-stations");
        RequestBinder(new RequestBinder());
    }

    public override async Task<Results<Ok<PagedResponse<PollingStationModel>>, ProblemDetails>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var specification = new ListPollingStationsSpecification(request.AddressFilter, request.Filter, request.PageSize, request.PageNumber);
        var csos = await _repository.ListAsync(specification, ct);
        var csosCount = await _repository.CountAsync(specification, ct);
        var result = csos.Select(x => new PollingStationModel
        {
            Id = x.Id,
            Address = x.Address,
            DisplayOrder = x.DisplayOrder,
            Tags = x.Tags.ToDictionary()
        }).ToList();

        return TypedResults.Ok(new PagedResponse<PollingStationModel>(result, csosCount, request.PageNumber, request.PageSize));
    }
}
