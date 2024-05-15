using Authorization.Policies;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.Notifications.Unsubscribe;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, NoContent>
{
    public override void Configure()
    {
        Post("/api/notifications:unsubscribe");
        DontAutoTag();
        Options(x => x.WithTags("notifications", "mobile"));
        Description(x => x.Accepts<Request>());
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<NoContent> ExecuteAsync(Request req, CancellationToken ct)
    {
        await context
            .NotificationTokens
            .Where(x => x.ObserverId == req.ObserverId)
            .ExecuteDeleteAsync(ct);

        return TypedResults.NoContent();
    }
}
