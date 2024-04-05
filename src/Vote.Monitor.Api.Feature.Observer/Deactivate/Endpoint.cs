namespace Vote.Monitor.Api.Feature.Observer.Deactivate;

public class Endpoint(IRepository<ObserverAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/observers/{id}:deactivate");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var observer = await repository.GetByIdAsync(req.Id, ct);
        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        observer.Deactivate();
        await repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
