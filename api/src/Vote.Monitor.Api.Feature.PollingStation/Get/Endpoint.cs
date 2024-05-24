using Vote.Monitor.Api.Feature.PollingStation.Helpers;

namespace Vote.Monitor.Api.Feature.PollingStation.Get;

public class Endpoint : Endpoint<Request, Results<Ok<PollingStationModel>, NotFound>>
{
    private readonly IReadRepository<PollingStationAggregate> _repository;

    public Endpoint(IReadRepository<PollingStationAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
    }

    public override async Task<Results<Ok<PollingStationModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var pollingStation = await _repository.FirstOrDefaultAsync(new GetPollingStationByIdSpecification(req.ElectionRoundId, req.Id), ct);

        if (pollingStation is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new PollingStationModel
        {
            Id = pollingStation.Id,
            Level1 = pollingStation.Level1,
            Level2 = pollingStation.Level2,
            Level3 = pollingStation.Level3,
            Level4 = pollingStation.Level4,
            Level5 = pollingStation.Level5,
            Number = pollingStation.Number,
            Address = pollingStation.Address,
            DisplayOrder = pollingStation.DisplayOrder,
            Tags = pollingStation.Tags.ToDictionary()
        });
    }
}
