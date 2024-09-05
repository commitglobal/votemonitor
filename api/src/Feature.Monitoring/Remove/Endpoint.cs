using Feature.Monitoring.Specifications;

namespace Feature.Monitoring.Remove;

public class Endpoint(IRepository<ElectionRoundAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound<string>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/monitoring-ngos/{ngoId}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
    }

    public override async Task<Results<NoContent, NotFound<string>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.SingleOrDefaultAsync(new GetElectionRoundByIdSpecification(req.ElectionRoundId), ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
        }

        var monitoringNgo = electionRound.MonitoringNgos.FirstOrDefault(x => x.NgoId == req.NgoId);

        if (monitoringNgo is null)
        {
            return TypedResults.NotFound("Ngo not found");
        }

        electionRound.RemoveMonitoringNgo(monitoringNgo);

        await repository.UpdateAsync(electionRound, ct);
        return TypedResults.NoContent();
    }
}
