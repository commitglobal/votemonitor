namespace Vote.Monitor.Api.Feature.Monitoring.RemoveObserver;

public class Endpoint(IRepository<ElectionRoundAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound<string>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{id}/monitoring-observers/{observerId}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
    }

    public override async Task<Results<NoContent, NotFound<string>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.GetByIdAsync(req.Id, ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
        }

        var observerIsMonitoringElection = electionRound.MonitoringObservers.Any(x => x.ObserverId == req.ObserverId);
        if (!observerIsMonitoringElection)
        {
            return TypedResults.NotFound("Requested observer does not monitor requested election round");
        }

        electionRound.RemoveMonitoringObserver(req.ObserverId);

        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
