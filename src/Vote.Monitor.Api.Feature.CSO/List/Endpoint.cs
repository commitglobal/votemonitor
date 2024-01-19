using Vote.Monitor.Api.Feature.CSO.Specifications;

namespace Vote.Monitor.Api.Feature.CSO.List;

public class Endpoint(IReadRepository<CSOAggregate> _repository) : Endpoint<Request, Results<Ok<PagedResponse<CSOModel>>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/csos");
    }

    public override async Task<Results<Ok<PagedResponse<CSOModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListCSOsSpecification(req);
        var csos = await _repository.ListAsync(specification, ct);
        var csosCount = await _repository.CountAsync(specification, ct);
        var result = csos.Select(x => new CSOModel
        {
            Id = x.Id,
            Name = x.Name,
            Status = x.Status,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn
        }).ToList();

        return TypedResults.Ok(new PagedResponse<CSOModel>(result, csosCount, req.PageNumber, req.PageSize));
    }
}
