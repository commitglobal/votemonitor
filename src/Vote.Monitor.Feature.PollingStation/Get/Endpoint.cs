namespace Vote.Monitor.Feature.PollingStation.Get;

public class Endpoint : Endpoint<Request, Results<Ok<PollingStationModel>, NotFound>>
{
    private readonly IReadRepository<PollingStationAggregate> _repository;

    public Endpoint(IReadRepository<PollingStationAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/{id:guid}");
    }

    public override async Task<Results<Ok<PollingStationModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var pollingStation = await _repository.GetByIdAsync(req.Id, ct);

        if (pollingStation is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new PollingStationModel
        {
            Id = pollingStation.Id,
            Address = pollingStation.Address,
            DisplayOrder = pollingStation.DisplayOrder,
            Tags = pollingStation.Tags.ToDictionary()
        });
    }
}
