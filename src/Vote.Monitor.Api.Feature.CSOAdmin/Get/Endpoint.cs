using Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

namespace Vote.Monitor.Api.Feature.CSOAdmin.Get;

public class Endpoint : Endpoint<Request, Results<Ok<CSOAdminModel>, NotFound>>
{
     readonly IReadRepository<CSOAdminAggregate> _repository;

    public Endpoint(IReadRepository<CSOAdminAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/csos/{CSOid}/admins/{id}");
    }

    public override async Task<Results<Ok<CSOAdminModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdminByIdSpecification(req.CSOId,req.Id);
        var csoAdmin = await _repository.SingleOrDefaultAsync(specification, ct);

        if (csoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new CSOAdminModel
        {
            Id = csoAdmin.Id,
            Name = csoAdmin.Name,
            Login = csoAdmin.Login,
            Status = csoAdmin.Status
        });

    }
}
