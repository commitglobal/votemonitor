namespace Vote.Monitor.ElectionRound.Delete;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
     readonly IRepository<ElectionRoundAggregate> _repository;

    public Endpoint(IRepository<ElectionRoundAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/api/election-rounds/{id}");
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
