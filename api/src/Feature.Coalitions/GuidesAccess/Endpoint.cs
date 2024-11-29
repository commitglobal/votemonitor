using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Feature.NgoCoalitions.GuidesAccess;

public class Endpoint(
    VoteMonitorContext context,
    IAuthorizationService authorizationService)
    : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/guides/{guideId}:access");
        DontAutoTag();
        Options(x => x.WithTags("coalitions"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User,
            new CoalitionLeaderRequirement(req.ElectionRoundId, req.CoalitionId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var guide = await context.ObserversGuides
            .Where(g =>
                g.Id == req.GuideId
                && g.MonitoringNgo.NgoId == req.NgoId
                && g.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                && !g.IsDeleted)
            .Select(g => new { Id = g.Id })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (guide is null)
        {
            return TypedResults.NotFound();
        }

        var coalition = await context.Coalitions
            .Where(c => c.ElectionRoundId == req.ElectionRoundId && c.Id == req.CoalitionId)
            .Include(x => x.GuideAccess.Where(ga => ga.GuideId == guide.Id && ga.MonitoringNgo.ElectionRoundId == req.ElectionRoundId))
            .Include(x => x.Memberships)
            .FirstOrDefaultAsync(ct);

        if (coalition is null)
        {
            return TypedResults.NotFound();
        }

        var requestNgoMembers = req.NgoMembersIds.Distinct().ToList();
        var coalitionMembersIds = coalition
            .Memberships
            .Select(x => x.MonitoringNgoId)
            .Distinct()
            .ToList();

        var coalitionMonitoringNgoIds = await context.MonitoringNgos
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && requestNgoMembers.Contains(x.NgoId)
                        && coalitionMembersIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(ct);

        var ngosGainedGuideAccess =
            coalitionMonitoringNgoIds.Where(x => coalition.GuideAccess.All(ga => ga.MonitoringNgoId != x))
                .Select(id => CoalitionGuideAccess.Create(coalition.Id, id, req.GuideId))
                .ToList();

        if (ngosGainedGuideAccess.Any())
        {
            await context.CoalitionGuideAccess.AddRangeAsync(ngosGainedGuideAccess, ct);
        }

        await context.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
