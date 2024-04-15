using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.MonitoringObservers.Tag;

public class Endpoint(IAuthorizationService authorizationService,
    VoteMonitorContext context) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers:tags");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Appends tags to selected monitoring observers";
            s.Description = "Results in a set of unique tags for selected observers";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        await context
            .MonitoringObservers
            .Where(x => req.MonitoringObserverIds.Contains(x.Id))
            .Where(x => x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .Where(x => x.MonitoringNgoId == req.MonitoringNgoId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.Tags, b => Postgres.Functions.ArrayUnique(b.Tags.Concat(req.Tags))), cancellationToken: ct);

        return TypedResults.NoContent();
    }
}
