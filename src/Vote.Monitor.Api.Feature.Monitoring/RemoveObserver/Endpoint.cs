using Vote.Monitor.Api.Feature.Monitoring.Specifications;

namespace Vote.Monitor.Api.Feature.Monitoring.RemoveObserver;

public class Endpoint(IRepository<MonitoringNgoAggregate> repository,
    IReadRepository<ObserverAggregate> observersRepository) : Endpoint<Request, Results<NoContent, NotFound<string>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/monitoring-ngos/{ngoId}/monitoring-observers/{observerId}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
    }

    public override async Task<Results<NoContent, NotFound<string>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var monitoringNgo = await repository.FirstOrDefaultAsync(new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        if (monitoringNgo == null)
        {
            return TypedResults.NotFound("Monitoring NGO not found");
        }

        var observer = await observersRepository.GetByIdAsync(req.ObserverId, ct);
        if (observer == null)
        {
            return TypedResults.NotFound("Observer not found");
        }

        monitoringNgo.RemoveMonitoringObserver(observer);

        await repository.UpdateAsync(monitoringNgo, ct);
        return TypedResults.NoContent();
    }
}
