using Feature.Review.Models;
using Feature.Review.Services;

namespace Feature.Review.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<RecordReviewState>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/reviews/{recordType}/{recordId}");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Gets a record's review stage, reviewers, severity and labels, as seen by the caller's NGO"; });
    }

    public override async Task<Results<Ok<RecordReviewState>, NotFound>> ExecuteAsync(Request req,
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

        var state = await context.GetReviewStateAsync(report.Target, ct);
        if (state is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(state);
    }
}
