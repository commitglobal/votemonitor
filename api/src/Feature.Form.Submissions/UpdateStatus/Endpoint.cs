using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.Form.Submissions.UpdateStatus;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    ICurrentUserProvider userProvider,
    ITimeProvider timeProvider)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/form-submissions/{id}:status");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s => { s.Summary = "Updates follow up status for a submission"; });

        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        
        var updatedInformation = await context.PollingStationInformation
            .Where(x => x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                        && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.FollowUpStatus, req.FollowUpStatus), cancellationToken: ct);

        var updatedSubmissions = await context.FormSubmissions
            .Where(x => x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                        && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.FollowUpStatus, req.FollowUpStatus), cancellationToken: ct);

        if (updatedInformation + updatedSubmissions > 0)
        {
            await context.AddFollowUpStatusActivityAsync(req.ElectionRoundId, req.NgoId, ReportRecordType.FormSubmission, req.Id,
                userProvider.GetUserId()!.Value, req.FollowUpStatus.Value, timeProvider.UtcNow, ct);
            await context.SaveChangesAsync(ct);
        }

        return TypedResults.NoContent();
    }
}
