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
        AllowAnonymous();
    }

    public override async Task<Results<Ok<CSOAdminModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdmin(req.CSOId,req.Id);
        var CSO = await _repository.GetBySpecAsync(specification, ct);

        if (CSO is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new CSOAdminModel
        {
            Name = CSO.Name,
            Login = CSO.Login,
            Password = CSO.Password,
            Status = CSO.Status
        });

    }
}
