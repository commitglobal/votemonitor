using Feature.Labels.Services;
using Feature.Review;
using Feature.Review.Services;

namespace Feature.Labels.Set;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    ILabelService labelService)
    : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/labels/{recordType}/{recordId}");
        DontAutoTag();
        Options(x => x.WithTags("labels"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Replaces a record's labels in the caller's NGO";
            s.Description =
                "NGO admins and staff can label any record their NGO sees, also in archived rounds. Labels are trimmed; " +
                "blank ones and case-insensitive duplicates are dropped, and a label the NGO already uses in the round " +
                "keeps that spelling. A real change is recorded once in the record's review history (LabelsChanged) " +
                "and notifies its reviewers.";
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

        await labelService.SetAsync(report, req.Labels, actor, ct);
        return TypedResults.NoContent();
    }
}
