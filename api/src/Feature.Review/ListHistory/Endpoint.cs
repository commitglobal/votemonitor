using Feature.Review.Models;
using Feature.Review.Services;

namespace Feature.Review.ListHistory;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<ReviewActivityModel[]>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/reviews/{recordType}/{recordId}/history");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Lists a record's review history in the caller's NGO, newest first"; });
    }

    public override async Task<Results<Ok<ReviewActivityModel[]>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        if (!await authorizationService.CanWorkOnReportsAsync(User, req.ElectionRoundId, req.RecordType))
        {
            return TypedResults.NotFound();
        }

        var report = await reviewService.ResolveAsync(req.ElectionRoundId, req.NgoId, req.RecordType, req.RecordId, ct);
        if (report is null)
        {
            return TypedResults.NotFound();
        }

        var activities = await context.ReviewActivities
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == report.Target.MonitoringNgoId
                        && x.RecordType == req.RecordType
                        && x.RecordId == req.RecordId)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync(ct);

        return TypedResults.Ok(activities.Select(ReviewActivityModel.FromEntity).ToArray());
    }
}
