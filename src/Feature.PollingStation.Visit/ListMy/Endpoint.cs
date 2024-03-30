using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Authorization.Policies;

namespace Feature.PollingStation.Visit.ListMy;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-station-visits:my");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-visit", "mobile"));
        Summary(s =>
        {
            s.Summary = "Lists visited polling stations of an observer";
            s.Description = "Polling station visits are based on polling station information / form answers / notes / attachments";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!result.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var visits = await context
            .PollingStationVisits
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.ObserverId == req.ObserverId)
            .Select(x => new VisitModel
            {
                PollingStationId = x.PollingStationId,
                VisitedAt = x.VisitedAt
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return TypedResults.Ok(new Response { Visits = visits });
    }
}
