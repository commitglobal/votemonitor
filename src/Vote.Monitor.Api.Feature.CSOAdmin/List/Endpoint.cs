using Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

namespace Vote.Monitor.Api.Feature.CSOAdmin.List;

public class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<CSOAdminModel>>, ProblemDetails>>
{
     readonly IReadRepository<CSOAdminAggregate> _repository;

    public Endpoint(IReadRepository<CSOAdminAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/csos/{CSOid}/admins");
    }

    public override async Task<Results<Ok<PagedResponse<CSOAdminModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListCSOAdminsSpecification(req);
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
