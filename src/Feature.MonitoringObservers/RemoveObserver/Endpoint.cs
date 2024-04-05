
using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.MonitoringObservers.RemoveObserver;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<MonitoringNgoAggregate> repository,
    IRepository<MonitoringObserverAggregate> monitoringObserversRepository
    ) : Endpoint<Request, Results<NoContent, NotFound<string>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers/{id}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
    }

    public override async Task<Results<NoContent, NotFound<string>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound("");
        }

        var monitoringNgo = await repository.FirstOrDefaultAsync(new GetMonitoringNgoWithObserversSpecification(req.ElectionRoundId, req.MonitoringNgoId), ct);

        if (monitoringNgo == null)
        {
            return TypedResults.NotFound("Monitoring NGO not found");
        }

        var monitoringObserver = monitoringNgo.MonitoringObservers.FirstOrDefault(x => x.Id == req.Id);
        if (monitoringObserver is not null)
        {
            await monitoringObserversRepository.DeleteAsync(monitoringObserver, ct);
        }
        return TypedResults.NoContent();
    }
}
