using Authorization.Policies;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.Notifications.MarkAsRead;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/notifications:read");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Summary(s => { s.Summary = "Read notifications for election round"; });
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await context
            .MonitoringObserverNotification
            .Where(x => x.MonitoringObserver.ObserverId == req.ObserverId
                        && req.NotificationIds.Contains(x.NotificationId)
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsRead, true), ct);

        await SendNoContentAsync(ct);
    }
}