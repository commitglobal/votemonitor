namespace Vote.Monitor.CSO.Activate;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
    private readonly IRepository<Domain.Entities.CSOAggregate.CSO> _repository;

    public Endpoint(IRepository<Domain.Entities.CSOAggregate.CSO> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/csos/{id:guid}:activate");
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var CSO = await _repository.GetByIdAsync(req.Id, ct);

        if (CSO is null)
        {
            return TypedResults.NotFound();
        }

        CSO.Activate();

        await _repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
