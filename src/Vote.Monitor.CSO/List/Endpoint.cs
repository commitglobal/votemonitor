using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.CSO.List;

public class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<CSOModel>>, ProblemDetails>>
{
    private readonly IReadRepository<Domain.Entities.CSOAggregate.CSO> _repository;

    public Endpoint(IReadRepository<Domain.Entities.CSOAggregate.CSO> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/csos");
        AllowAnonymous();
        Summary(s => s.ExampleRequest = new Request()
        {
            PageNumber = 1,
            PageSize = 100
        });
    }

    public override async Task<Results<Ok<PagedResponse<CSOModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListCSOsSpecification(req.NameFilter, req.Status, req.PageSize, req.PageNumber);
        var CSOs = await _repository.ListAsync(specification, ct);
        var CSOsCount = await _repository.CountAsync(specification, ct);
        var result = CSOs.Select(x => new CSOModel()
        {
            Id = x.Id,
            Name = x.Name,
            Status = x.Status
        }).ToList();

        return TypedResults.Ok(new PagedResponse<CSOModel>(result, CSOsCount, req.PageNumber, req.PageSize));
    }
}
