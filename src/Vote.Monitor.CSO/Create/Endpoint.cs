namespace Vote.Monitor.CSO.Create;

public class Endpoint : Endpoint<Request, Results<Ok<CSOModel>, Conflict<ProblemDetails>>>
{
    private readonly IRepository<Domain.Entities.CSOAggregate.CSO> _repository;

    public Endpoint(IRepository<Domain.Entities.CSOAggregate.CSO> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/csos");
    }

    public override async Task<Results<Ok<CSOModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOByNameSpecification(req.Name);
        var hasCSOWithSameName = await _repository.AnyAsync(specification, ct);

        if (hasCSOWithSameName)
        {
            AddError(r => r.Name, "A CSO with same name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var cso = new Domain.Entities.CSOAggregate.CSO(req.Name);
        await _repository.AddAsync(cso, ct);

        return TypedResults.Ok(new CSOModel
        {
            Id = cso.Id,
            Name = cso.Name,
            Status = cso.Status
        });
    }
}
