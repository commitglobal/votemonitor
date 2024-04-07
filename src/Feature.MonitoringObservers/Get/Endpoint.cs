using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Specifications;
using Microsoft.AspNetCore.Authorization;

namespace Feature.MonitoringObservers.Get;

public class Endpoint(IAuthorizationService authorizationService, IRepository<MonitoringObserverAggregate> repository) : Endpoint<Request, Results<Ok<MonitoringObserverModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers/{id}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Gets monitoring observer details";
        });
    }

    public override async Task<Results<Ok<MonitoringObserverModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringObserver = await repository.FirstOrDefaultAsync(new GetMonitoringObserverModelSpecification(req.ElectionRoundId, req.MonitoringNgoId, req.Id), ct);

        if (monitoringObserver is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(monitoringObserver);
    }
}
