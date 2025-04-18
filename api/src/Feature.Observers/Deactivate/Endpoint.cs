using Feature.Observers.Specifications;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Feature.Observers.Deactivate;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<ObserverAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Put("/api/observers/{id}:deactivate");
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(x => { x.Description = "Deactivates account of an observer"; });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetObserverByIdSpecification(req.Id);
        var observer = await repository.SingleOrDefaultAsync(specification, ct);

        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        observer.ApplicationUser.Deactivate();
        var result = await userManager.UpdateAsync(observer.ApplicationUser);

        if (!result.Succeeded)
        {
            AddError(x => x.Id, result.GetAllErrors());
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        return TypedResults.NoContent();
    }
}
