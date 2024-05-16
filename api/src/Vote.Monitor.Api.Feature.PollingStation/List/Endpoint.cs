using Vote.Monitor.Api.Feature.PollingStation.Helpers;
using Vote.Monitor.Api.Feature.PollingStation.Specifications;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.List;
public class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<PollingStationModel>>, ProblemDetails>>
{
    private readonly IReadRepository<PollingStationAggregate> _repository;

    public Endpoint(IReadRepository<PollingStationAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations:list");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
    }

    public override async Task<Results<Ok<PagedResponse<PollingStationModel>>, ProblemDetails>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var specification = new ListPollingStationsSpecification(request);
        var pollingStations = await _repository.ListAsync(specification, ct);
        var pollingStationsCount = await _repository.CountAsync(specification, ct);
        var result = pollingStations.Select(x => new PollingStationModel
        {
            Id = x.Id,
            Level1 = x.Level1,
            Level2 = x.Level2,
            Level3 = x.Level3,
            Level4 = x.Level4,
            Level5 = x.Level5,
            Number = x.Number,
            Address = x.Address,
            DisplayOrder = x.DisplayOrder,
            Tags = x.Tags.ToDictionary()
        }).ToList();

        return TypedResults.Ok(new PagedResponse<PollingStationModel>(result, pollingStationsCount, request.PageNumber, request.PageSize));
    }
}
