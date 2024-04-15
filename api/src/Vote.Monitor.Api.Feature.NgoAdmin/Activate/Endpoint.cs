using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Api.Feature.NgoAdmin.Specifications;
using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Feature.NgoAdmin.Activate;

public class Endpoint(UserManager<ApplicationUser> userManager,
    IReadRepository<NgoAdminAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/ngos/{ngoId}/admins/{id}:activate");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
        Summary(x => { x.Description = "Activates account of a ngo admin"; });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetNgoAdminByIdSpecification(req.NgoId, req.Id);
        var ngoAdmin = await repository.SingleOrDefaultAsync(specification, ct);

        if (ngoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        ngoAdmin.ApplicationUser.Activate();
        var result = await userManager.UpdateAsync(ngoAdmin.ApplicationUser);

        if (!result.Succeeded)
        {
            AddError(x => x.Id, result.GetAllErrors());
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        return TypedResults.NoContent();
    }
}
