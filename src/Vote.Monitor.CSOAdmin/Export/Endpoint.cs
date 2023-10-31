using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSOAdmin.Export;

public class Endpoint : EndpointWithoutRequest<Results<Ok<CSOAdminModel>, NotFound>>
{
     readonly IReadRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> _repository;

    public Endpoint(IReadRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/csos/{CSOid:guid}/admins:export");
    }

    public override async Task<Results<Ok<CSOAdminModel>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
