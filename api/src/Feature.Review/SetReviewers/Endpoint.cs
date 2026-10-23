using Feature.Review.Services;

namespace Feature.Review.SetReviewers;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    ICurrentUserRoleProvider roleProvider) : Endpoint<Request, Results<NoContent, NotFound, ForbidHttpResult, ValidationProblem>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/reviews/{recordType}/{recordId}:reviewers");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Replaces a record's reviewers in the caller's NGO";
            s.Description = "Reviewers must be active admins or staff of the caller's NGO. Staff can only add or remove themselves.";
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

        var result = await reviewService.SetReviewersAsync(report, req.UserIds, actor, roleProvider.IsNgoAdmin(), ct);
        switch (result.Outcome)
        {
            case ReviewOutcome.Forbidden:
                return TypedResults.Forbid();
            case ReviewOutcome.InvalidReviewers:
                AddError(x => x.UserIds,
                    $"Not active admins or staff of your NGO: {string.Join(", ", result.InvalidUserIds)}");
                return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
            default:
                return TypedResults.NoContent();
        }
    }
}
