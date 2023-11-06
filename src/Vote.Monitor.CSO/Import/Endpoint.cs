namespace Vote.Monitor.CSO.Import;

public class Endpoint : Endpoint<Request, Results<Ok<CSOModel>, NotFound>>
{
    private readonly IReadRepository<CSOAggregate> _repository;

    public Endpoint(IReadRepository<CSOAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/csos:import");
        DontAutoTag();
        Options(x => x.WithTags("csos"));
    }

    public override async Task<Results<Ok<CSOModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
