using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.ElectionRound.List;

public class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<ElectionRoundModel>>, ProblemDetails>>
{
     readonly IReadRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> _repository;

    public Endpoint(IReadRepository<Domain.Entities.ElectionRoundAggregate.ElectionRound> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds");
    }

    public override async Task<Results<Ok<PagedResponse<ElectionRoundModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
