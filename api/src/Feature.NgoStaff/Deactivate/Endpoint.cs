using Authorization.Policies;
using Feature.NgoStaff.Specifications;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Feature.NgoStaff.Deactivate;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<NgoStaffAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/ngos/{ngoId}/staff/{id}:deactivate");
        DontAutoTag();
        Options(x => x.WithTags("ngo-staff"));
        Description(x => x.Accepts<Request>());
        Summary(x => { x.Description = "Deactivates account of a ngo staff"; });
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new GetNgoStaffByIdSpecification(req.NgoId, req.Id);
        var ngoAdmin = await repository.SingleOrDefaultAsync(specification, ct);

        if (ngoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        ngoAdmin.ApplicationUser.Deactivate();
        var result = await userManager.UpdateAsync(ngoAdmin.ApplicationUser);

        if (!result.Succeeded)
        {
            AddError(x => x.Id, result.GetAllErrors());
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        return TypedResults.NoContent();
    }
}
