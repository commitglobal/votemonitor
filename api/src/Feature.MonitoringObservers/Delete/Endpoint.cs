using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers.Delete;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringObserverAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/monitoring-observers/{id}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Deletes a pending monitoring observer";
            s.Description = "Permanently removes a monitoring observer that has not accepted the invitation yet";
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

        var monitoringObserver = await repository.FirstOrDefaultAsync(
            new GetMonitoringObserverSpecification(req.ElectionRoundId, req.NgoId, req.Id), ct);

        if (monitoringObserver is null || monitoringObserver.Status != MonitoringObserverStatus.Pending)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(monitoringObserver, ct);

        return TypedResults.NoContent();
    }
}
