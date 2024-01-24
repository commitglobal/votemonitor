namespace Vote.Monitor.Api.Feature.Monitoring.RemoveNgo;

public class Endpoint(IRepository<ElectionRoundAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound<string>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{id}/monitoring-ngos/{ngoId}");
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

        var ngoIsMonitoringElection = electionRound.MonitoringNgos.Any(x=>x.NgoId == req.NgoId);
        if (!ngoIsMonitoringElection)
        {
            return TypedResults.NotFound("Requested NGO does not monitor requested election round");
        }

        electionRound.RemoveMonitoringNgo(req.NgoId);

        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
