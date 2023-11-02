namespace Vote.Monitor.CSO.Export;

public class Endpoint : EndpointWithoutRequest<Results<Ok<CSOModel>, NotFound>>
{
    private readonly IReadRepository<CSOAggregate> _repository;

    public Endpoint(IReadRepository<CSOAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/csos:export");
    }

    public override async Task<Results<Ok<CSOModel>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
