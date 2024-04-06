using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.MonitoringObservers.Suspend;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<MonitoringNgoAggregate> repository,
    IRepository<MonitoringObserverAggregate> monitoringObserversRepository
    ) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers/{id}:suspend");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Suspends monitoring observer account";
            s.Description = "Changes status of monitoring observer to Suspended";
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

        var monitoringNgo = await repository.FirstOrDefaultAsync(new GetMonitoringNgoWithObserversSpecification(req.ElectionRoundId, req.MonitoringNgoId), ct);

        var monitoringObserver = monitoringNgo!.MonitoringObservers.FirstOrDefault(x => x.Id == req.Id);
        if (monitoringObserver is not null)
        {
            monitoringObserver.Suspend();
            await monitoringObserversRepository.UpdateAsync(monitoringObserver, ct);
        }

        return TypedResults.NoContent();
    }
}
