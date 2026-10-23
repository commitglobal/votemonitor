using Feature.Review.Models;
using Feature.Review.Services;

namespace Feature.Review.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<ReviewWorkflowModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/reviews/{recordType}/{recordId}");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Gets a record's review stage and reviewers, as seen by the caller's NGO"; });
    }

    public override async Task<Results<Ok<ReviewWorkflowModel>, NotFound>> ExecuteAsync(Request req,
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

        var reviewerIds = report.Reviewers.Select(x => x.UserId).ToList();
        var reviewers = await context.Users
            .Where(x => reviewerIds.Contains(x.Id))
            .OrderBy(x => x.DisplayName)
            .Select(x => new ReviewerModel { Id = x.Id, DisplayName = x.DisplayName, Status = x.Status })
            .ToArrayAsync(ct);

        return TypedResults.Ok(new ReviewWorkflowModel
        {
            Stage = ReviewStageModel.FromEntity(report.Stage),
            StageChangedAt = report.Workflow?.StageChangedAt,
            ReviewedAt = report.Workflow?.ReviewedAt,
            Reviewers = reviewers
        });
    }
}
