using Authorization.Policies;
using Feature.NgoStaff.Specifications;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Feature.NgoStaff.Delete;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<NgoStaffAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Delete("/api/ngos/{ngoId}/staff/{id}");
        DontAutoTag();
        Options(x => x.WithTags("ngo-staff"));
        Summary(x => { x.Description = "Permanently delete account of a ngo staff"; });
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new GetNgoStaffByIdSpecification(req.NgoId, req.Id);
        var ngoAdmin = await repository.SingleOrDefaultAsync(specification, ct);

        if (ngoAdmin == null)
        {
            return TypedResults.NotFound();
        }

        var result = await userManager.DeleteAsync(ngoAdmin.ApplicationUser);
        if (!result.Succeeded)
        {
            AddError(x => x.Id, result.GetAllErrors());
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        return TypedResults.NoContent();
    }
}
