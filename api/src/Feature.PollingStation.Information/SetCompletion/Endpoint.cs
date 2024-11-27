using Authorization.Policies;
using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.PollingStation.Information.SetCompletion;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information:setCompletion");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information", "mobile"));
        Summary(s => { s.Summary = "Updates completion status status for a submission"; });

        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        await context.PollingStationInformation
            .Where(x => x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.PollingStationId == req.PollingStationId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsCompleted, req.IsCompleted), cancellationToken: ct);

        return TypedResults.NoContent();
    }
}
