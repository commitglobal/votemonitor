using Authorization.Policies;
using Feature.NgoAdmins.Specifications;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Feature.NgoAdmins.Delete;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<NgoAdminAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Delete("/api/ngos/{ngoId}/admins/{id}");
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
        Summary(x => { x.Description = "Permanently delete account of a ngo admin"; });
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new GetNgoAdminByIdSpecification(req.NgoId, req.Id);
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
