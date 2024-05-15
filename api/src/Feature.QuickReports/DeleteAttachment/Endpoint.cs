using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.QuickReports.DeleteAttachment;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports", "mobile"));
        Summary(s =>
        {
            s.Summary = "Deletes a quick report and it's attachments";
        });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        // Mark as deleted so our background job will delete it. also change the id in order to allow recreation with same id
        await context
            .QuickReportAttachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.QuickReportId == req.QuickReportId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(qra => qra.IsDeleted, true)
                .SetProperty(qra => qra.Id, Guid.NewGuid()), cancellationToken: ct);

        return TypedResults.NoContent();
    }
}
