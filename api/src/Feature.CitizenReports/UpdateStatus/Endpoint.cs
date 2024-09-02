using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.CitizenReports.UpdateStatus;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/citizen-reports/{electionRoundId}/submission/{id}:status");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Summary(s =>
        {
            s.Summary = "Updates follow up status for a citizen report";
        });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        await context.CitizenReports
            .Where(x => x.Form.MonitoringNgo.NgoId == req.NgoId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.FollowUpStatus, req.FollowUpStatus), cancellationToken: ct);
        
        return TypedResults.NoContent();
    }
}
