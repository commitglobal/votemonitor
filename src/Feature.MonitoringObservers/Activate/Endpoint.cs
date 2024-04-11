using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.MonitoringObservers.Activate;

public class Endpoint(IAuthorizationService authorizationService, IRepository<MonitoringObserverAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers/{id}:activate");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Activates monitoring observer account";
            s.Description = "Changes status of monitoring observer to Active";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringObserver = await repository.FirstOrDefaultAsync(new GetMonitoringObserverSpecification(req.ElectionRoundId, req.MonitoringNgoId, req.Id), ct);

        if (monitoringObserver is null)
        {
            return TypedResults.NotFound();
        }

        monitoringObserver.Activate();
        await repository.UpdateAsync(monitoringObserver, ct);

        return TypedResults.NoContent();
    }
}
