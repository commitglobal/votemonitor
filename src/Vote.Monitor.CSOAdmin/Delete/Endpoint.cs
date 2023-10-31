using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.CSOAdmin.Specifications;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSOAdmin.Delete;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
     readonly IRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> _repository;

    public Endpoint(IRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/api/csos/{CSOid:guid}/admins/{id:guid}");
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var csoAdmin = await _repository.SingleOrDefaultAsync(new GetCSOAdminByIdSpecification(req.CSOId, req.Id), ct);

        if (csoAdmin == null)
        {
            return TypedResults.NotFound();
        }

        await _repository.DeleteAsync(csoAdmin, ct);
        return TypedResults.NoContent();
    }
}
