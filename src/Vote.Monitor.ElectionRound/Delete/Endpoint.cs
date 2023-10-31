using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.ElectionRound.Delete;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
     readonly IRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> _repository;

    public Endpoint(IRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/api/election-rounds/{id:guid}");
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
