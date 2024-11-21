using Feature.NgoCoalitions.Services;

namespace Feature.NgoCoalitions.Delete;

public class Endpoint(VoteMonitorContext context, IFormSubmissionsCleanupService cleanupService)
    : Endpoint<Request, Results<NoContent, NotFound, Conflict>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");
        DontAutoTag();
        Options(x => x.WithTags("coalitions"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, Conflict>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var coalition = await context.Coalitions
            .Include(x => x.Memberships)
            .Where(x => x.Id == req.CoalitionId && x.ElectionRoundId == req.ElectionRoundId)
            .FirstOrDefaultAsync(ct);

        if (coalition == null)
        {
            return TypedResults.NotFound();
        }

        // members data should be purged but coalition leader keeps their data
        var membersToRemove = coalition
            .Memberships
            .Where(x => x.MonitoringNgoId != coalition.LeaderId)
            .Select(x => x.MonitoringNgoId)
            .ToList();

        // Delete orphaned data
        if (membersToRemove.Any())
        {
            await Task.WhenAll(membersToRemove.Select(monitoringNgoId =>
                cleanupService.CleanupFormSubmissionsAsync(req.ElectionRoundId, req.CoalitionId, monitoringNgoId)));
        }

        context.Coalitions.Remove(coalition);
        await context.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
