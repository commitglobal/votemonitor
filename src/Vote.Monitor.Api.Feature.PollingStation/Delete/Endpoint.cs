namespace Vote.Monitor.Api.Feature.PollingStation.Delete;
public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    private readonly IRepository<PollingStationAggregate> _repository;

    public Endpoint(IRepository<PollingStationAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-stations/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var pollingStation = await _repository.GetByIdAsync(req.Id, ct);

        if (pollingStation is null)
        {
            return TypedResults.NotFound();
        }

        await _repository.DeleteAsync(pollingStation, ct);

        return TypedResults.NoContent();
    }
}
