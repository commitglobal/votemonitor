namespace Vote.Monitor.Api.Feature.CSO.Get;

public class Endpoint : Endpoint<Request, Results<Ok<CSOModel>, NotFound>>
{
    private readonly IReadRepository<CSOAggregate> _repository;

    public Endpoint(IReadRepository<CSOAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/csos/{id}");
    }

    public override async Task<Results<Ok<CSOModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var CSO = await _repository.GetByIdAsync(req.Id, ct);

        if (CSO is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new CSOModel
        {
            Id = CSO.Id,
            Name = CSO.Name,
            Status = CSO.Status,
            CreatedOn = CSO.CreatedOn,
            LastModifiedOn = CSO.LastModifiedOn
        });
    }
}
