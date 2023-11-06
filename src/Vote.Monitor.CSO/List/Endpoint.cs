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
    }

    public override async Task<Results<Ok<PagedResponse<CSOModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListCSOsSpecification(req.NameFilter, req.Status, req.PageSize, req.PageNumber);
        var csos = await _repository.ListAsync(specification, ct);
        var csosCount = await _repository.CountAsync(specification, ct);
        var result = csos.Select(x => new CSOModel
        {
            Id = x.Id,
            Name = x.Name,
            Status = x.Status
        }).ToList();

        return TypedResults.Ok(new PagedResponse<CSOModel>(result, csosCount, req.PageNumber, req.PageSize));
    }
}
