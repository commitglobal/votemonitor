using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.ElectionRound.Create;

public class Endpoint : Endpoint<Request, Results<Ok<ElectionRoundModel>, Conflict<ProblemDetails>>>
{
     readonly IRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> _repository;

    public Endpoint(IRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> repository)
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
