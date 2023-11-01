namespace Vote.Monitor.CSO.Import;

public class Endpoint : Endpoint<Request, Results<Ok<CSOModel>, NotFound>>
{
    private readonly IReadRepository<Domain.Entities.CSOAggregate.CSO> _repository;

    public Endpoint(IReadRepository<Domain.Entities.CSOAggregate.CSO> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/csos:import");
    }

    public override async Task<Results<Ok<CSOModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
