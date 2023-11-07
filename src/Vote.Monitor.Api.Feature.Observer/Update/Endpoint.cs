namespace Vote.Monitor.Api.Feature.Observer.Update;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
     readonly IRepository<ObserverAggregate> _repository;

    public Endpoint(IRepository<ObserverAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/observers/{id}");
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var observer = await _repository.GetByIdAsync(req.Id, ct);

        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        observer.UpdateDetails(req.Name);
        await _repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
