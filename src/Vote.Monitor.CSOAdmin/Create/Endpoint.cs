using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.CSOAdmin.Specifications;
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
    }

    public override async Task<Results<Ok<CSOAdminModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdminByLoginSpecification(req.CSOId, req.Login);
        var hasCSOAdminWithSameLogin = await _repository.AnyAsync(specification, ct);

        if (hasCSOAdminWithSameLogin)
        {
            AddError(r => r.Name, "A CSO admin with same login already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var csoAdmin = new Domain.Entities.ApplicationUserAggregate.CSOAdmin(req.CSOId, req.Name, req.Login, req.Password);
        await _repository.AddAsync(csoAdmin, ct);

        return TypedResults.Ok(new CSOAdminModel
        {
            Id = csoAdmin.Id,
            Name = csoAdmin.Name,
            Login = csoAdmin.Login,
            Status = csoAdmin.Status
        });

    }
}
