using Feature.Review.Services;

namespace Feature.Review.Stages.Delete;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    VoteMonitorContext context,
    ITimeProvider timeProvider)
    : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/review-stages/{id}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Deletes a review stage, moving its records to another stage";
            s.Description = "The default stage can't be deleted. A stage in use needs moveTo; each moved report gets a history entry. The stage is removed from the reviewer rules it triggers.";
        });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem, Conflict<ProblemDetails>>> ExecuteAsync(
        Request req, CancellationToken ct)
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
        var actor = await reviewService.GetActorAsync(req.UserId, ct);
        if (stage is null || actor is null)
        {
            return TypedResults.NotFound();
        }

        if (await reviewService.IsArchivedAsync(req.ElectionRoundId, ct))
        {
            AddError(x => x.ElectionRoundId, "Election round is archived");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        if (stage.IsDefault)
        {
            AddError(x => x.Id, "The default stage can't be deleted; make another stage the default first");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures, StatusCodes.Status409Conflict));
        }

        var workflows = await context.ReviewWorkflows.Where(x => x.StageId == stage.Id).ToListAsync(ct);
        var target = stages.FirstOrDefault(x => x.Id == req.MoveTo);
        if (workflows.Count > 0 && target is null)
        {
            AddError(x => x.MoveTo, "The stage is in use; choose another of your stages to move its reports to");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures, StatusCodes.Status409Conflict));
        }

        var now = timeProvider.UtcNow;
        foreach (var workflow in workflows)
        {
            workflow.SetStage(target!, now);
            context.ReviewActivities.Add(ReviewActivity.StageChanged(
                new ReviewTarget(workflow.ElectionRoundId, workflow.MonitoringNgoId, workflow.RecordType,
                    workflow.RecordId),
                actor,
                stage,
                target!,
                now));
        }

        var rules = await context.ReviewerRules
            .Where(x => x.MonitoringNgoId == stage.MonitoringNgoId && x.StageIds.Contains(stage.Id))
            .ToListAsync(ct);
        foreach (var rule in rules)
        {
            rule.RemoveStage(stage.Id);
        }

        context.ReviewStages.Remove(stage);
        await context.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
