namespace Vote.Monitor.Api.Feature.Observer.Delete;

public class Endpoint(IRepository<ObserverAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/observers/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var observer = await repository.GetByIdAsync(req.Id, ct);
        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(observer, ct);

        return TypedResults.NoContent();
    }
}
