namespace Vote.Monitor.Api.Feature.Observer.Delete;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
    readonly IRepository<ObserverAggregate> _repository;

    public Endpoint(IRepository<ObserverAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/api/observers/{id}");
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var observer = await _repository.GetByIdAsync(req.Id, ct);
        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        await _repository.DeleteAsync(observer, ct);

        return TypedResults.NoContent();
    }
}
