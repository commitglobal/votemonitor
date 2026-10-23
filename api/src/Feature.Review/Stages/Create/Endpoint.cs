using Feature.Review.Models;
using Feature.Review.Services;

namespace Feature.Review.Stages.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<ReviewStageModel>, NotFound, ValidationProblem, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/review-stages");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s => { s.Summary = "Adds a review stage at the end of its review status"; });
    }

    public override async Task<Results<Ok<ReviewStageModel>, NotFound, ValidationProblem, Conflict<ProblemDetails>>>
        ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        if (monitoringNgoId is null)
        {
            return TypedResults.NotFound();
        }

        if (await reviewService.IsArchivedAsync(req.ElectionRoundId, ct))
        {
            AddError(x => x.ElectionRoundId, "Election round is archived");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var stages = await context.ReviewStages.Where(x => x.MonitoringNgoId == monitoringNgoId).ToListAsync(ct);
        var name = req.Name.Trim();
        if (stages.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            AddError(x => x.Name, "A stage with this name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures, StatusCodes.Status409Conflict));
        }

        var sequence = stages.Where(x => x.Status == req.Status).Select(x => x.Sequence).DefaultIfEmpty(0).Max()
                       + ReviewStage.SequenceStep;
        var stage = ReviewStage.Create(monitoringNgoId.Value, name, req.Description?.Trim(), req.Status, sequence);
        context.ReviewStages.Add(stage);
        await context.SaveChangesAsync(ct);

        return TypedResults.Ok(ReviewStageModel.FromEntity(stage));
    }
}
