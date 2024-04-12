using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Feature.Observer.Delete;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<ObserverAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Delete("/api/observers/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(x => { x.Description = "Permanently deletes account of an observer"; });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetObserverByIdSpecification(req.Id);
        var observer = await repository.SingleOrDefaultAsync(specification, ct);

        if (observer == null)
        {
            return TypedResults.NotFound();
        }

        var result = await userManager.DeleteAsync(observer.ApplicationUser);
        if (!result.Succeeded)
        {
            AddError(x => x.Id, result.GetAllErrors());
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        return TypedResults.NoContent();
    }
}
