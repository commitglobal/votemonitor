namespace Vote.Monitor.Api.Feature.ElectionRound.Create;

public class Endpoint : Endpoint<Request, Results<Ok<ElectionRoundModel>, Conflict<ProblemDetails>>>
{
     readonly IRepository<ElectionRoundAggregate> _repository;

    public Endpoint(IRepository<ElectionRoundAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/election-rounds");
    }

    public override async Task<Results<Ok<ElectionRoundModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
