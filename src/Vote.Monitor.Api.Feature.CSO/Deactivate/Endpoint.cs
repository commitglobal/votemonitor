namespace Vote.Monitor.Api.Feature.CSO.Deactivate;

public class Endpoint(IRepository<CSOAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/csos/{id}:deactivate");
        Description(x => x.Accepts<Request>());
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var CSO = await repository.GetByIdAsync(req.Id, ct);

        if (CSO is null)
        {
            return TypedResults.NotFound();
        }

        CSO.Deactivate();

        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
