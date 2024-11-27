using Feature.NgoCoalitions.Models;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Feature.NgoCoalitions.Update;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<CoalitionModel>, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");
        DontAutoTag();
        Options(x => x.WithTags("coalitions"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<CoalitionModel>, NotFound, ProblemDetails>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var coalition = await context.Coalitions
            .Include(x => x.Memberships)
            .ThenInclude(x => x.MonitoringNgo)
            .Include(x => x.Leader)
            .ThenInclude(x => x.Ngo)
            .Include(x => x.ElectionRound)
            .FirstOrDefaultAsync(x => x.Id == req.CoalitionId && x.ElectionRoundId == req.ElectionRoundId, ct);

        if (coalition is null)
        {
            return TypedResults.NotFound();
        }

        var ngoIds = req.NgoMembersIds.Distinct().ToList();
        var newMembers = await context.Ngos.Where(x => ngoIds.Contains(x.Id)).ToListAsync(ct);

        var coalitionMembers = await context
            .MonitoringNgos
            .Include(x => x.Ngo)
            .Where(x => ngoIds.Contains(x.NgoId) && x.ElectionRoundId == req.ElectionRoundId)
            .ToListAsync(ct);

        var ngosToAddAsMonitoringNgos = newMembers.Where(ngo => coalitionMembers.All(x => x.NgoId != ngo.Id)).ToList();

        foreach (var ngo in ngosToAddAsMonitoringNgos)
        {
            var monitoringNgo = coalition.ElectionRound.AddMonitoringNgo(ngo);
            coalitionMembers.Add(monitoringNgo);
            context.MonitoringNgos.Add(monitoringNgo);
        }

        coalition.Update(req.CoalitionName,
            coalitionMembers.Select(x => CoalitionMembership.Create(req.ElectionRoundId, req.CoalitionId, x.Id)));

        await context.SaveChangesAsync(ct);
        return TypedResults.Ok(CoalitionModel.FromEntity(coalition));
    }
}
