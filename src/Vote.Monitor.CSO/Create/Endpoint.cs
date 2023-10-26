using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.CSO.Specifications;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSO.Create;

public class Endpoint : Endpoint<Request, Results<Ok<CSOModel>, Conflict<ProblemDetails>>>
{
    private readonly IRepository<Domain.Entities.CSOAggregate.CSO> _repository;

    public Endpoint(IRepository<Domain.Entities.CSOAggregate.CSO> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/csos");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<CSOModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOByName(req.Name);
        var hasCSOWithSameName = await _repository.AnyAsync(specification, ct);

        if (hasCSOWithSameName)
        {
            AddError(r => r.Name, "A CSO with same name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var CSO = new Domain.Entities.CSOAggregate.CSO(req.Name);
        await _repository.AddAsync(CSO, ct);

        return TypedResults.Ok(new CSOModel
        {
            Id = CSO.Id,
            Name = CSO.Name,
            Status = CSO.Status
        });
    }
}
