using Feature.Labels.Models;
using Feature.Labels.Services;
using Feature.Review;
using Feature.Review.Services;

namespace Feature.Labels.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    ILabelService labelService) : Endpoint<Request, Results<Ok<LabelsModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/labels/{recordType}/{recordId}");
        DontAutoTag();
        Options(x => x.WithTags("labels"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Gets a record's labels in the caller's NGO, A to Z"; });
    }

    public override async Task<Results<Ok<LabelsModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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

        var labels = await labelService.GetAsync(report.Target, ct);
        return TypedResults.Ok(new LabelsModel(labels));
    }
}
