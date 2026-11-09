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

        ReviewStage? parent = null;
        if (req.ParentStageId is { } parentId)
        {
            parent = stages.FirstOrDefault(x => x.Id == parentId);
            if (parent is null || parent.Id == stage.Id || parent.ParentStageId is not null)
            {
                AddError(x => x.ParentStageId, "Pick one of your other top-level stages; sub-stages can't have sub-stages");
                return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
            }

            if (stages.Any(x => x.ParentStageId == stage.Id))
            {
                AddError(x => x.ParentStageId, "A stage with sub-stages can't become a sub-stage");
                return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
            }
        }

        // A sub-stage takes its parent's status; moved under another parent or status, it goes last there.
        var status = parent?.Status ?? req.Status;
        var samePlace = status == stage.Status && parent?.Id == stage.ParentStageId;
        var siblings = parent is null
            ? stages.Where(x => x.Id != stage.Id && x.ParentStageId is null && x.Status == status)
            : stages.Where(x => x.Id != stage.Id && x.ParentStageId == parent.Id);
        var sequence = req.Sequence
                       ?? (samePlace
                           ? stage.Sequence
                           : siblings.Select(x => x.Sequence).DefaultIfEmpty(0).Max() + ReviewStage.SequenceStep);
        stage.Update(name, req.Description?.Trim(), status, sequence, parent?.Id, req.Color, req.Icon);

        // Sub-stages follow their parent to its new status.
        foreach (var child in stages.Where(x => x.ParentStageId == stage.Id))
        {
            child.MoveToStatus(status);
        }

        // Reports already in this stage keep their reviewed time as it was; only later moves set or clear it.
        await context.SaveChangesAsync(ct);

        return TypedResults.Ok(ReviewStageModel.FromEntity(stage));
    }
}
