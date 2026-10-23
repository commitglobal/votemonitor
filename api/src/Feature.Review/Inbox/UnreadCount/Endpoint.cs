namespace Feature.Review.Inbox.UnreadCount;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/inbox:unread-count");
        DontAutoTag();
        Options(x => x.WithTags("inbox"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Counts the caller's unread inbox items"; });
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var count = await context.UserNotifications.CountAsync(x => x.UserId == req.UserId && x.ReadAt == null, ct);
        return TypedResults.Ok(new Response(count));
    }
}
