namespace Vote.Monitor.Api.Feature.Ngo.Activate;

public class Endpoint(IRepository<NgoAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{

    public override void Configure()
    {
        Post("/api/ngos/{id}:activate");
        Description(x => x.Accepts<Request>());
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var ngo = await repository.GetByIdAsync(req.Id, ct);

        if (ngo is null)
        {
            return TypedResults.NotFound();
        }

        ngo.Activate();

        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
