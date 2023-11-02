namespace Vote.Monitor.Observer.Delete;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
    readonly IRepository<Domain.Entities.ApplicationUserAggregate.Observer> _repository;

    public Endpoint(IRepository<Domain.Entities.ApplicationUserAggregate.Observer> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/api/observers{id:guid}");
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
