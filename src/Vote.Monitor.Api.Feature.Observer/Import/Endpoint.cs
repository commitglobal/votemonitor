namespace Vote.Monitor.Api.Feature.Observer.Import;

public class Endpoint : Endpoint<Request, Results<Ok<ObserverModel>, NotFound>>
{
     readonly IReadRepository<ObserverAggregate> _repository;

    public Endpoint(IReadRepository<ObserverAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/observers:import");
        DontAutoTag();
        Options(x => x.WithTags("observers"));
    }

    public override async Task<Results<Ok<ObserverModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
