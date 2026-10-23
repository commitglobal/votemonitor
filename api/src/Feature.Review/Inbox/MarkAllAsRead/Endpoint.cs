namespace Feature.Review.Inbox.MarkAllAsRead;

public class Endpoint(VoteMonitorContext context, ITimeProvider timeProvider) : Endpoint<Request, NoContent>
{
    public override void Configure()
    {
        Post("/api/inbox:read-all");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("inbox"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Marks all of the caller's inbox items as read"; });
    }

    public override async Task<NoContent> ExecuteAsync(Request req, CancellationToken ct)
    {
        var now = timeProvider.UtcNow;
        await context.UserNotifications
            .Where(x => x.UserId == req.UserId && x.ReadAt == null)
            .ExecuteUpdateAsync(x => x.SetProperty(n => n.ReadAt, now), ct);
        return TypedResults.NoContent();
    }
}
