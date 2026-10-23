namespace Feature.Review.Inbox.MarkAsRead;

public class Endpoint(VoteMonitorContext context, ITimeProvider timeProvider)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/inbox/{id}:read");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("inbox"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Marks one of the caller's inbox items as read"; });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var notification =
            await context.UserNotifications.FirstOrDefaultAsync(x => x.Id == req.Id && x.UserId == req.UserId, ct);
        if (notification is null)
        {
            return TypedResults.NotFound();
        }

        notification.MarkAsRead(timeProvider.UtcNow);
        await context.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
