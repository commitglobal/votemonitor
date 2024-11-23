using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Feature.NgoCoalitions.FormAccess;

public class Endpoint(
    VoteMonitorContext context,
    IAuthorizationService authorizationService)
    : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access");
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

        var form = await context.Forms
            .Where(f => f.ElectionRoundId == req.ElectionRoundId
                        && f.Id == req.FormId
                        && f.ElectionRoundId == req.ElectionRoundId
                        && f.MonitoringNgo.NgoId == req.NgoId
                        && f.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .Select(f => new { f.Id, f.Status })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        var coalition = await context.Coalitions
            .Where(c => c.ElectionRoundId == req.ElectionRoundId && c.Id == req.CoalitionId)
            .Include(x => x.FormAccess.Where(fa => fa.FormId == form.Id && fa.Form.ElectionRoundId == req.ElectionRoundId))
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

        var ngosGainedFormAccess =
            coalitionMonitoringNgoIds.Where(x => coalition.FormAccess.All(fa => fa.MonitoringNgoId != x))
                .Select(id => CoalitionFormAccess.Create(coalition.Id, id, req.FormId))
                .ToList();

        if (ngosGainedFormAccess.Any())
        {
            await context.CoalitionFormAccess.AddRangeAsync(ngosGainedFormAccess, ct);
        }

        await context.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
