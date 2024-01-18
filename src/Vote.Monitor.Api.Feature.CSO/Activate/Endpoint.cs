namespace Vote.Monitor.Api.Feature.CSO.Activate;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
    private readonly IRepository<CSOAggregate> _repository;

    public Endpoint(IRepository<CSOAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/csos/{id}:activate");
        Description(x => x.Accepts<Request>());
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
