namespace Vote.Monitor.CSOAdmin.Import;

public class Endpoint : Endpoint<Request, Results<Ok<CSOAdminModel>, NotFound>>
{
     readonly IReadRepository<CSOAdminAggregate> _repository;

    public Endpoint(IReadRepository<CSOAdminAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/csos/{CSOid:guid}/admins:import");
    }

    public override async Task<Results<Ok<CSOAdminModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
