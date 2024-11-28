using Authorization.Policies;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.QuickReports.UpdateStatus;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, NoContent>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/quick-reports/{id}:status");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Summary(s =>
        {
            s.Summary = "Updates follow up status for a quick report";
        });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<NoContent> ExecuteAsync(Request req, CancellationToken ct)
    {
        await context.QuickReports
            .Where(x => x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                        && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.FollowUpStatus, req.FollowUpStatus), cancellationToken: ct);

        return TypedResults.NoContent();
    }
}
