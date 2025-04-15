using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Feature.PollingStations.GetPollingStationsVersion;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations:version");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets current version of polling stations for an election round";
            s.Description = "Cache key changes every time any polling station changes";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringObserverRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var electionRound = await context.ElectionRounds
            .Where(x => x.Id == req.ElectionRoundId)
            .Select(x => new { x.PollingStationsVersion, x.Id })
            .FirstOrDefaultAsync(ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new Response
        {
            ElectionRoundId = electionRound.Id,
            CacheKey = electionRound.PollingStationsVersion.ToString()
        });
    }
}
