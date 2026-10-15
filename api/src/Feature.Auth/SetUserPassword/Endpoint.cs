using Authorization.Policies;
using Feature.Auth.Specifications;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Auth.SetUserPassword;

public class Endpoint(
    IRepository<ApplicationUser> repository,
    IRepository<MonitoringObserver> monitoringObserverRepository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/auth/set-password");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.AspNetUserId, ct);
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

        await repository.UpdateAsync(user, ct);
        await monitoringObserverRepository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
