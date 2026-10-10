using Authorization.Policies;
using Feature.NgoStaff.Specifications;

namespace Feature.NgoStaff.Get;

public class Endpoint(IRepository<NgoStaffAggregate> repository)
    : Endpoint<Request, Results<Ok<NgoStaffModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/ngos/{ngoId}/staff/{id}");
        DontAutoTag();
        Options(x => x.WithTags("ngo-staff"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<NgoStaffModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetNgoStaffByIdSpecification(req.NgoId, req.Id);
        var ngoAdmin = await repository.SingleOrDefaultAsync(specification, ct);

        if (ngoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new NgoStaffModel
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
