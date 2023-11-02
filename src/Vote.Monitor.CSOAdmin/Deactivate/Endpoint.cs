namespace Vote.Monitor.CSOAdmin.Deactivate;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
     readonly IRepository<CSOAdminAggregate> _repository;

    public Endpoint(IRepository<CSOAdminAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/csos/{CSOid:guid}/admins/{id:guid}:deactivate");
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdminByIdSpecification(req.CSOId, req.Id);
        var csoAdmin = await _repository.SingleOrDefaultAsync(specification, ct);

        if (csoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        csoAdmin.Deactivate();
        await _repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
