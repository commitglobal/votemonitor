using Feature.Review.Services;

namespace Feature.Review.SetStage;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    ICurrentUserRoleProvider roleProvider) : Endpoint<Request, Results<NoContent, NotFound, ForbidHttpResult, ValidationProblem>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/reviews/{recordType}/{recordId}:stage");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Moves a report to another of the caller's NGO's stages";
            s.Description = "NGO admins can move any report; staff only the records they review.";
        });
    }

    public override async Task<Results<NoContent, NotFound, ForbidHttpResult, ValidationProblem>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        if (!await authorizationService.CanWorkOnReportsAsync(User, req.ElectionRoundId, req.RecordType))
        {
            return TypedResults.NotFound();
        }

        var report = await reviewService.ResolveAsync(req.ElectionRoundId, req.NgoId, req.RecordType, req.RecordId, ct);
        var actor = await reviewService.GetActorAsync(req.UserId, ct);
        if (report is null || actor is null)
        {
            return TypedResults.NotFound();
        }

        var outcome = await reviewService.SetStageAsync(report, req.StageId, actor, roleProvider.IsNgoAdmin(), ct);
        switch (outcome)
        {
            case ReviewOutcome.Forbidden:
                return TypedResults.Forbid();
            case ReviewOutcome.InvalidStage:
                AddError(x => x.StageId, "The stage doesn't belong to your NGO in this election round");
                return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
            default:
                return TypedResults.NoContent();
        }
    }
}
