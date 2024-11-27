using Feature.NgoCoalitions.Models;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Feature.NgoCoalitions.Create;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Results<Ok<CoalitionModel>, Conflict, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/coalitions");
        DontAutoTag();
        Options(x => x.WithTags("coalitions"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<CoalitionModel>, Conflict, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var electionRound = await context.ElectionRounds.FirstOrDefaultAsync(x => x.Id == req.ElectionRoundId, ct);
        if (electionRound == null)
        {
            return TypedResults.NotFound();
        }

        var coalitionWithSameNameExists = await context
            .Coalitions
            .AnyAsync(x => EF.Functions.ILike(x.Name, req.CoalitionName) && x.ElectionRoundId == req.ElectionRoundId,
                ct);

        if (coalitionWithSameNameExists)
        {
            return TypedResults.Conflict();
        }

        var ngoIds = req.NgoMembersIds.Union([req.LeaderId]).Distinct().ToList();
        var ngos = await context.Ngos.Where(x => ngoIds.Contains(x.Id)).ToListAsync(ct);

        var monitoringNgos = await context
            .MonitoringNgos
            .Include(x => x.Ngo)
            .Where(x => ngoIds.Contains(x.NgoId) && x.ElectionRoundId == req.ElectionRoundId)
            .ToListAsync(ct);

        var ngosToAddAsMonitoringNgos = ngos.Where(ngo => monitoringNgos.All(x => x.NgoId != ngo.Id)).ToList();

        foreach (var ngo in ngosToAddAsMonitoringNgos)
        {
            var monitoringNgo = electionRound.AddMonitoringNgo(ngo);
            monitoringNgos.Add(monitoringNgo);
            context.MonitoringNgos.Add(monitoringNgo);
        }

        var coalition = Coalition.Create(electionRound,
            req.CoalitionName,
            monitoringNgos.First(x => x.NgoId == req.LeaderId),
            monitoringNgos);

        context.Coalitions.Add(coalition);

        await context.SaveChangesAsync(ct);

        return TypedResults.Ok(CoalitionModel.FromEntity(coalition));
    }
}
