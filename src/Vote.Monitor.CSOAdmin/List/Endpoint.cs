using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Core.Models;
using Vote.Monitor.CSOAdmin.Specifications;
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
    }

    public override async Task<Results<Ok<PagedResponse<CSOAdminModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListCSOAdminsSpecification(req.NameFilter, req.Status, req.PageSize, req.PageNumber);
        var csos = await _repository.ListAsync(specification, ct);
        var csosCount = await _repository.CountAsync(specification, ct);
        var result = csos.Select(x => new CSOAdminModel
        {
            Id = x.Id,
            Name = x.Name,
            Login = x.Login,
            Status = x.Status
        }).ToList();

        return TypedResults.Ok(new PagedResponse<CSOAdminModel>(result, csosCount, req.PageNumber, req.PageSize));
    }
}
