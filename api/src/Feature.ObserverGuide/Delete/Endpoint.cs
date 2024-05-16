using Authorization.Policies.Requirements;
using Feature.ObserverGuide.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.ObserverGuide.Delete;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<ObserverGuideAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/observer-guide/{id}");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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

        observerGuide.Delete();

        await repository.UpdateAsync(observerGuide, ct);

        return TypedResults.NoContent();
    }
}
