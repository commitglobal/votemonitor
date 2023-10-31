using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.CSOAdmin.Specifications;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSOAdmin.Activate;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
     readonly IRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> _repository;

    public Endpoint(IRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/csos/{CSOid:guid}/admins/{id:guid}:activate");
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdminByIdSpecification(req.CSOId, req.Id);
        var csoAdmin = await _repository.FirstOrDefaultAsync(specification, ct);

        if (csoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        csoAdmin.Activate();
        await _repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
