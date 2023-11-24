namespace Vote.Monitor.Api.Feature.Observer.Import;

public class Endpoint : Endpoint<Request, Results<Ok<Response>, NotFound, ProblemDetails>>
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
        AllowFileUploads();
    }

    public override async Task<Results<Ok<Response>, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
