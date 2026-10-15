using Authorization.Policies;
using Feature.Auth.Specifications;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Auth.SetUserPassword;

public class Endpoint(
    IUserStore<ApplicationUser> userStore,
    IRepository<MonitoringObserver> monitoringObserverRepository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/auth/set-password");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var user = await userStore.FindByIdAsync(request.AspNetUserId.ToString(), ct);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        // setting a password activates the account (ngo admin, ngo staff or observer)
        user.AcceptInvite(request.NewPassword);

        var monitoringObservers =
            await monitoringObserverRepository.ListAsync(new ListMonitoringObserverSpecification(user.Id), ct);

        foreach (var monitoringObserver in monitoringObservers)
        {
            monitoringObserver.Activate();
        }

        await userStore.UpdateAsync(user, ct);
        await monitoringObserverRepository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
