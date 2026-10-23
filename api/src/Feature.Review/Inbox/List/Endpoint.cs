using Feature.Review.Models;

namespace Feature.Review.Inbox.List;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Ok<PagedResponse<InboxItemModel>>>
{
    public override void Configure()
    {
        Get("/api/inbox");
        DontAutoTag();
        Options(x => x.WithTags("inbox"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Lists the caller's inbox, newest first"; });
    }

    public override async Task<Ok<PagedResponse<InboxItemModel>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var query = context.UserNotifications
            .AsNoTracking()
            .Where(x => x.UserId == req.UserId && (req.UnreadOnly != true || x.ReadAt == null));

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.CreatedOn)
            .Skip((req.PageNumber - 1) * req.PageSize)
            .Take(req.PageSize)
            .Include(x => x.Activity)
            .Join(context.ElectionRounds, x => x.ElectionRoundId, er => er.Id,
                (x, er) => new { Notification = x, er.Title })
            .ToListAsync(ct);

        var models = items.Select(x => new InboxItemModel
        {
            Id = x.Notification.Id,
            Reason = x.Notification.Reason,
            CreatedOn = x.Notification.CreatedOn,
            ReadAt = x.Notification.ReadAt,
            ElectionRoundId = x.Notification.ElectionRoundId,
            ElectionRoundTitle = x.Title,
            RecordType = x.Notification.RecordType,
            RecordId = x.Notification.RecordId,
            Activity = ReviewActivityModel.FromEntity(x.Notification.Activity)
        }).ToList();

        return TypedResults.Ok(new PagedResponse<InboxItemModel>(models, totalCount, req.PageNumber, req.PageSize));
    }
}
