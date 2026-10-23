using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.IncidentReports.UpdateStatus;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    ICurrentUserProvider userProvider,
    ITimeProvider timeProvider) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/incident-reports/{id}:status");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Summary(s => { s.Summary = "Updates follow up status for a incident report"; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        
        var updated = await context.IncidentReports
            .Where(x => x.Form.MonitoringNgo.NgoId == req.NgoId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Form.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgoId == x.Form.MonitoringNgoId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.FollowUpStatus, req.FollowUpStatus), cancellationToken: ct);

        if (updated > 0)
        {
            await context.AddFollowUpStatusActivityAsync(req.ElectionRoundId, req.NgoId, ReportRecordType.IncidentReport, req.Id,
                userProvider.GetUserId()!.Value, req.FollowUpStatus.Value, timeProvider.UtcNow, ct);
            await context.SaveChangesAsync(ct);
        }

        return TypedResults.NoContent();
    }
}
