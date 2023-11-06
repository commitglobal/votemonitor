namespace Vote.Monitor.Api.Feature.ElectionRound.Get;

public class Endpoint : Endpoint<Request, Results<Ok<ElectionRoundModel>, NotFound>>
{
     readonly IReadRepository<ElectionRoundAggregate> _repository;

    public Endpoint(IReadRepository<ElectionRoundAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{id}");
    }

    public override async Task<Results<Ok<ElectionRoundModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
