using Vote.Monitor.Api.Feature.NgoAdmin.Specifications;

namespace Vote.Monitor.Api.Feature.NgoAdmin.Get;

public class Endpoint(IRepository<Domain.Entities.NgoAdminAggregate.NgoAdmin> _repository) : Endpoint<Request, Results<Ok<NgoAdminModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/ngos/{ngoId}/admins/{id}");
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
    }

    public override async Task<Results<Ok<NgoAdminModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetNgoAdminByIdSpecification(req.NgoId, req.Id);
        var ngoAdmin = await _repository.SingleOrDefaultAsync(specification, ct);

        if (ngoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new NgoAdminModel
        {
            Id = ngoAdmin.Id,
            FirstName = ngoAdmin.ApplicationUser.FirstName,
            LastName = ngoAdmin.ApplicationUser.LastName,
            Email = ngoAdmin.ApplicationUser.Email,
            Status = ngoAdmin.ApplicationUser.Status,
            CreatedOn = ngoAdmin.CreatedOn,
            LastModifiedOn = ngoAdmin.LastModifiedOn
        });

    }
}
