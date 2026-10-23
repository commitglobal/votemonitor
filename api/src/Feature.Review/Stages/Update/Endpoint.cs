using Feature.Review.Models;
using Feature.Review.Services;

namespace Feature.Review.Stages.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<ReviewStageModel>, NotFound, ValidationProblem, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/review-stages/{id}");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s => { s.Summary = "Renames a review stage, or moves it within or to another review status"; });
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
        var stages = await context.ReviewStages.Where(x => x.MonitoringNgoId == monitoringNgoId).ToListAsync(ct);
        var stage = stages.FirstOrDefault(x => x.Id == req.Id);
        if (stage is null)
        {
            return TypedResults.NotFound();
        }

        if (await reviewService.IsArchivedAsync(req.ElectionRoundId, ct))
        {
            AddError(x => x.ElectionRoundId, "Election round is archived");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var name = req.Name.Trim();
        if (stages.Any(x => x.Id != stage.Id && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            AddError(x => x.Name, "A stage with this name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures, StatusCodes.Status409Conflict));
        }

        var sequence = req.Sequence
                       ?? (req.Status == stage.Status
                           ? stage.Sequence
                           : stages.Where(x => x.Status == req.Status).Select(x => x.Sequence).DefaultIfEmpty(0).Max()
                             + ReviewStage.SequenceStep);
        stage.Update(name, req.Description?.Trim(), req.Status, sequence);

        // Reports already in this stage keep their reviewed time as it was; only later moves set or clear it.
        await context.SaveChangesAsync(ct);

        return TypedResults.Ok(ReviewStageModel.FromEntity(stage));
    }
}
