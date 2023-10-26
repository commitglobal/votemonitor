using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSO.Get;

public class Endpoint : Endpoint<Request, Results<Ok<CSOModel>, NotFound>>
{
    private readonly IReadRepository<Domain.Entities.CSOAggregate.CSO> _repository;

    public Endpoint(IReadRepository<Domain.Entities.CSOAggregate.CSO> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/csos/{id:guid}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<CSOModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var CSO = await _repository.GetByIdAsync(req.Id, ct);

        if (CSO is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new CSOModel
        {
            Id = CSO.Id,
            Name = CSO.Name,
            Status = CSO.Status
        });
    }
}
