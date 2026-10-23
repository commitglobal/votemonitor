using Feature.Review.Models;

namespace Feature.Review.Stages.List;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<ReviewStageModel[]>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/review-stages");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Lists the caller's NGO's review stages in the round, in review order"; });
    }

    public override async Task<Results<Ok<ReviewStageModel[]>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var stages = await context.ReviewStages
            .AsNoTracking()
            .Where(x => x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId && x.MonitoringNgo.NgoId == req.NgoId)
            .ToListAsync(ct);

        return TypedResults.Ok(stages.InReviewOrder().Select(ReviewStageModel.FromEntity).ToArray());
    }
}
