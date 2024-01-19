using Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

namespace Vote.Monitor.Api.Feature.CSOAdmin.Get;

public class Endpoint(IRepository<CSOAdminAggregate> _repository) : Endpoint<Request, Results<Ok<CSOAdminModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/csos/{csoid}/admins/{id}");
    }

    public override async Task<Results<Ok<CSOAdminModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdminByIdSpecification(req.CSOId, req.Id);
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
            Status = csoAdmin.Status,
            CreatedOn = csoAdmin.CreatedOn,
            LastModifiedOn = csoAdmin.LastModifiedOn
        });

    }
}
