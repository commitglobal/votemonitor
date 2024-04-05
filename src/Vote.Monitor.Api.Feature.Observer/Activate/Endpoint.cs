namespace Vote.Monitor.Api.Feature.Observer.Activate;

public class Endpoint(IRepository<ObserverAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/observers/{id}:activate");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var observer = await repository.GetByIdAsync(req.Id, ct);
        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        observer.Activate();
        await repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
