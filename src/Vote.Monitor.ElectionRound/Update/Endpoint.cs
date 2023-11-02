namespace Vote.Monitor.ElectionRound.Update;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
     readonly IRepository<ElectionRoundAggregate> _repository;

    public Endpoint(IRepository<ElectionRoundAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/elections-round/{id:guid}");
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
