using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Feature.Observer.Update;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<ObserverAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Put("/api/observers/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var observer = await repository.GetByIdAsync(req.Id, ct);

        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        observer.ApplicationUser.UpdateDetails(req.FirstName, req.LastName, req.PhoneNumber);

        var result = await userManager.UpdateAsync(observer.ApplicationUser);
        if (!result.Succeeded)
        {
            AddError(x => x.Id, result.GetAllErrors());
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        return TypedResults.NoContent();
    }
}
