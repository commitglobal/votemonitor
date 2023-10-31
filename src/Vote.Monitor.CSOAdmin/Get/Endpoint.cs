using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.CSOAdmin.Specifications;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSOAdmin.Get;

public class Endpoint : Endpoint<Request, Results<Ok<CSOAdminModel>, NotFound>>
{
     readonly IReadRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> _repository;

    public Endpoint(IReadRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/csos/{CSOid:guid}/admins/{id:guid}");
    }

    public override async Task<Results<Ok<CSOAdminModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdminByIdSpecification(req.CSOId,req.Id);
        var csoAdmin = await _repository.FirstOrDefaultAsync(specification, ct);

        if (csoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new CSOAdminModel
        {
            Id = csoAdmin.Id,
            Name = csoAdmin.Name,
            Login = csoAdmin.Login,
            Status = csoAdmin.Status
        });

    }
}
