using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Feature.MonitoringObservers.Update;

public class Endpoint(IAuthorizationService authorizationService, IRepository<MonitoringObserverAggregate> repository, IUserStore<ApplicationUser> userStore) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-observers/{id}");
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

        var monitoringObserver = await repository.FirstOrDefaultAsync(new GetMonitoringObserverSpecification(req.ElectionRoundId, req.NgoId, req.Id), ct);

        if (monitoringObserver is null)
        {
            return TypedResults.NotFound();
        }

        monitoringObserver.Update(req.Status, req.Tags);
        await repository.UpdateAsync(monitoringObserver, ct);

        var applicationUser = await userStore.FindByIdAsync(monitoringObserver.ObserverId.ToString(), ct);
        if (applicationUser is null)
        {
            return TypedResults.NotFound();
        }
        applicationUser.UpdateDetails(req.FirstName, req.LastName, req.PhoneNumber);

        await userStore.UpdateAsync(applicationUser, ct);

        return TypedResults.NoContent();
    }
}
