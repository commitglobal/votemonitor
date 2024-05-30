using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Form.Submissions.Any;

public class Endpoint(IRepository<FormSubmission> repository, IAuthorizationService authorizationService) : Endpoint<Request, Results<Ok, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:any");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s =>
        {
            s.Summary = "Returns 200 if there are submissions for this polling station 404 if not";
        });
    }

    public override async Task<Results<Ok, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            TypedResults.NotFound();
        }

        var specification = new GetFormSubmissionForObserverSpecification(req.ElectionRoundId, req.ObserverId, [req.PollingStationId]);
        var hasSubmissionsForPollingStation = await repository.AnyAsync(
            specification,
            ct);

        return hasSubmissionsForPollingStation ? TypedResults.Ok() : TypedResults.NotFound();
    }
}
