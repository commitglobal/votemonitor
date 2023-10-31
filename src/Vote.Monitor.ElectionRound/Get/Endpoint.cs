using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.ElectionRound.Get;

public class Endpoint : Endpoint<Request, Results<Ok<ElectionRoundModel>, NotFound>>
{
     readonly IReadRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> _repository;

    public Endpoint(IReadRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{id:guid}");
    }

    public override async Task<Results<Ok<ElectionRoundModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
