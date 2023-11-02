namespace Vote.Monitor.ElectionRound.Export;

public class Endpoint : EndpointWithoutRequest<Results<Ok<ElectionRoundModel>, NotFound>>
{
     readonly IReadRepository<ElectionRoundAggregate> _repository;

    public Endpoint(IReadRepository<ElectionRoundAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds:export");
    }

    public override async Task<Results<Ok<ElectionRoundModel>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
