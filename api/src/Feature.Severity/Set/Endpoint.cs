using Feature.Review;
using Feature.Review.Services;
using Feature.Severity.Services;

namespace Feature.Severity.Set;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    ISeverityService severityService,
    ICurrentUserRoleProvider roleProvider)
    : Endpoint<Request, Results<NoContent, NotFound, ForbidHttpResult, ValidationProblem>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/severities/{recordType}/{recordId}");
        DontAutoTag();
        Options(x => x.WithTags("severities"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Sets a record's severity in the caller's NGO";
            s.Description = "NGO admins can change any record's severity; staff only on the records they review.";
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

        var outcome = await severityService.SetAsync(report, req.Severity, actor, roleProvider.IsNgoAdmin(), ct);
        return outcome == SetSeverityOutcome.Forbidden ? TypedResults.Forbid() : TypedResults.NoContent();
    }
}
