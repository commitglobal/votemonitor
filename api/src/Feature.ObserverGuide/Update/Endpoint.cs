using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.ObserverGuide.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.ObserverGuide.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<ObserverGuideAggregate> repository)
    : Endpoint<Request, Results<Ok<ObserverGuideModel>, NotFound, NoContent>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/observer-guide/{id}");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<ObserverGuideModel>, NotFound, NoContent>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetObserverGuideByIdSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var observerGuide = await repository.FirstOrDefaultAsync(specification, ct);

        if (observerGuide == null)
        {
            return TypedResults.NotFound();
        }

        observerGuide.UpdateTitle(req.Title);

        await repository.UpdateAsync(observerGuide, ct);

        return TypedResults.NoContent();
    }
}
