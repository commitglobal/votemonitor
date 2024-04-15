using Authorization.Policies.Requirements;
using Feature.ObserverGuide.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;

namespace Feature.ObserverGuide.Delete;

public class Endpoint(IAuthorizationService authorizationService,
    ICurrentUserRoleProvider currentUserRoleProvider, 
    IRepository<ObserverGuideAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/observer-guide/{id}");
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

        var ngoId = await currentUserRoleProvider.GetNgoId();
        var specification = new GetObserverGuideSpecification(ngoId, req.Id);
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
