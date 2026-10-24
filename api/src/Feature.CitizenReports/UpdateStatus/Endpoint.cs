using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.CitizenReports.UpdateStatus;

public class Endpoint(VoteMonitorContext context, ICurrentUserProvider userProvider, ITimeProvider timeProvider) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/citizen-reports/{id}:status");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Summary(s => { s.Summary = "Updates follow up status for a citizen report"; });

        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var updated = await context.CitizenReports
            .Where(x => x.Form.MonitoringNgo.NgoId == req.NgoId
                        && x.Form.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.FollowUpStatus, req.FollowUpStatus), cancellationToken: ct);

        if (updated > 0)
        {
            await context.AddFollowUpStatusActivityAsync(req.ElectionRoundId, req.NgoId, ReportRecordType.CitizenReport, req.Id,
                userProvider.GetUserId()!.Value, req.FollowUpStatus.Value, timeProvider.UtcNow, ct);
            await context.SaveChangesAsync(ct);
        }

        return TypedResults.NoContent();
    }
}
