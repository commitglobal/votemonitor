using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSOAdmin.Import;

public class Endpoint : Endpoint<Request, Results<Ok<CSOAdminModel>, NotFound>>
{
     readonly IReadRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> _repository;

    public Endpoint(IReadRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/csos/{CSOid:guid}/admins:import");
    }

    public override async Task<Results<Ok<CSOAdminModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
