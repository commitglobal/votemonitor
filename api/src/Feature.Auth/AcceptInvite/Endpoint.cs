using Feature.Auth.Specifications;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Auth.AcceptInvite;

public class Endpoint(IRepository<ApplicationUser> repository,
    IRepository<MonitoringObserver> monitoringObserverRepository) : Endpoint<Request, NoContent>
{
    public override void Configure()
    {
        Post("/api/auth/accept-invite");
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(Request request, CancellationToken ct)
    {
        var user = await repository.FirstOrDefaultAsync(new GetByInvitationCode(request.InvitationToken), ct);

        if (user is null)
        {
            return TypedResults.NoContent();
        }
        user.AcceptInvite(request.Password);

        // just in case they were invited multiple times
        var monitoringObservers = await monitoringObserverRepository.ListAsync(new ListMonitoringObserverSpecification(user.Id), ct);

        foreach (var monitoringObserver in monitoringObservers)
        {
            monitoringObserver.Activate();
        }

        await repository.UpdateAsync(user, ct);
        await monitoringObserverRepository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
