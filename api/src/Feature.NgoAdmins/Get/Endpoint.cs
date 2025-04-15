using Authorization.Policies;
using Feature.NgoAdmins.Specifications;

namespace Feature.NgoAdmins.Get;

public class Endpoint(IRepository<NgoAdminAggregate> repository)
    : Endpoint<Request, Results<Ok<NgoAdminModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/ngos/{ngoId}/admins/{id}");
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<NgoAdminModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetNgoAdminByIdSpecification(req.NgoId, req.Id);
        var ngoAdmin = await repository.SingleOrDefaultAsync(specification, ct);

        if (ngoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new NgoAdminModel
        {
            Id = ngoAdmin.Id,
            FirstName = ngoAdmin.ApplicationUser.FirstName,
            LastName = ngoAdmin.ApplicationUser.LastName,
            Email = ngoAdmin.ApplicationUser.Email!,
            PhoneNumber = ngoAdmin.ApplicationUser.PhoneNumber,
            Status = ngoAdmin.ApplicationUser.Status,
            CreatedOn = ngoAdmin.CreatedOn,
            LastModifiedOn = ngoAdmin.LastModifiedOn
        });
    }
}
