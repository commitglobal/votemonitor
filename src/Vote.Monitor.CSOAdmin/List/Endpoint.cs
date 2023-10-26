using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSOAdmin.List;

public class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<CSOAdminModel>>, ProblemDetails>>
{
     readonly IReadRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> _repository;

    public Endpoint(IReadRepository<Domain.Entities.ApplicationUserAggregate.CSOAdmin> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/csos/{CSOid:guid}/admins");
        AllowAnonymous();
        Summary(s => s.ExampleRequest = new Request()
        {
            PageNumber = 1,
            PageSize = 100
        });
    }

    public override async Task<Results<Ok<PagedResponse<CSOAdminModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
