namespace Vote.Monitor.Api.Feature.CSO.Activate;

public class Endpoint(IRepository<CSOAggregate> _repository) : Endpoint<Request, Results<NoContent, NotFound>>
{

    public override void Configure()
    {
        Put("/api/csos/{id}:activate");
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
