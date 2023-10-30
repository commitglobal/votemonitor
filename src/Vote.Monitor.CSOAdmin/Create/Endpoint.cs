using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.CSOAdmin.Specifications;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSOAdmin.Create;

public class Endpoint : Endpoint<Request, Results<Ok<CSOAdminModel>, Conflict<ProblemDetails>>>
{
     readonly IRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> _repository;

    public Endpoint(IRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/csos/{CSOid:guid}/admins");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<CSOAdminModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdminByName(req.CSOId, req.Name);
        var hasCSOAdminWithSameName = await _repository.AnyAsync(specification, ct);

        if (hasCSOAdminWithSameName)
        {
            AddError(r => r.Name, "A CSOAdmin admin with same name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var CSOAdmin = new Domain.Entities.ApplicationUserAggregate.CSOAdmin(req.CSOId, req.Name, req.Login, req.Password, UserRole.CSOAdmin);
        await _repository.AddAsync(CSOAdmin, ct);

        return TypedResults.Ok(new CSOAdminModel
        {
            Name = CSOAdmin.Name,
            Login = CSOAdmin.Login,
            Password = CSOAdmin.Password,
            Status = CSOAdmin.Status
        });

    }
}
