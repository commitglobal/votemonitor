using Feature.Review;
using Feature.Review.Services;
using Feature.Severity.Models;
using Feature.Severity.Services;

namespace Feature.Severity.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    ISeverityService severityService) : Endpoint<Request, Results<Ok<SeverityModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/severities/{recordType}/{recordId}");
        DontAutoTag();
        Options(x => x.WithTags("severities"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Gets a record's severity, as seen by the caller's NGO"; });
    }

    public override async Task<Results<Ok<SeverityModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        if (!await authorizationService.CanWorkOnReportsAsync(User, req.ElectionRoundId, req.RecordType))
        {
            return TypedResults.NotFound();
        }

        var report = await reviewService.ResolveAsync(req.ElectionRoundId, req.NgoId, req.RecordType, req.RecordId, ct);
        if (report is null)
        {
            return TypedResults.NotFound();
        }

        var severity = await severityService.GetAsync(report.Target, ct);
        return TypedResults.Ok(new SeverityModel
        {
            Severity = severity?.Severity ?? SeverityLevel.None,
            ChangedAt = severity?.ChangedAt
        });
    }
}
