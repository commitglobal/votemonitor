using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.ElectionRound.Update;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
     readonly IRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> _repository;

    public Endpoint(IRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> repository)
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
