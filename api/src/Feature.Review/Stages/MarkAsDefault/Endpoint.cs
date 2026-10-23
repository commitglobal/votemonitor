using Feature.Review.Services;

namespace Feature.Review.Stages.MarkAsDefault;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    VoteMonitorContext context) : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/review-stages/{id}:default");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Makes a review stage the one new records start in";
            s.Description = "Reports still in the previous default stage stay there.";
        });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req,
        CancellationToken ct)
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

        var previousDefault = stages.First(x => x.IsDefault);
        if (previousDefault.Id == stage.Id)
        {
            return TypedResults.NoContent();
        }

        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(ct);

            await reviewService.MaterializeDefaultStageAsync(req.ElectionRoundId, req.NgoId, previousDefault, ct);

            // Two saves: the partial unique index allows only one default at a time.
            previousDefault.UnmarkAsDefault();
            await context.SaveChangesAsync(ct);
            stage.MarkAsDefault();
            await context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        });

        return TypedResults.NoContent();
    }
}
