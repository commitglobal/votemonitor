using Feature.Review;
using Feature.Review.Services;
using Feature.Severity.Services;

namespace Feature.Severity.Set;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    ISeverityService severityService)
    : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
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
            s.Description = "NGO admins and staff can change any record's severity.";
        });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(
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

        await severityService.SetAsync(report, req.Severity, actor, ct);
        return TypedResults.NoContent();
    }
}
