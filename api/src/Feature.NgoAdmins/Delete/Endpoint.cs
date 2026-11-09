using Authorization.Policies;
using Feature.NgoAdmins.Specifications;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.SavedViewAggregate;

namespace Feature.NgoAdmins.Delete;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<NgoAdminAggregate> repository,
    VoteMonitorContext context) : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
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

        var userId = ngoAdmin.ApplicationUser.Id;
        var result = await userManager.DeleteAsync(ngoAdmin.ApplicationUser);
        if (!result.Succeeded)
        {
            AddError(x => x.Id, result.GetAllErrors());
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        // Their private saved views go with the account; shared ones stay for the NGO.
        await context.DeletePrivateViewsAsync(userId, ct);

        return TypedResults.NoContent();
    }
}
