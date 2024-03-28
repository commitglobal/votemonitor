using Authorization.Policies;
using Vote.Monitor.Api.Feature.Monitoring.Specifications;

namespace Vote.Monitor.Api.Feature.Monitoring.GetMonitoringObservers;
public class Endpoint(IReadRepository<MonitoringObserverAggregate> repository) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers");
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var monitoringObservers = await repository.ListAsync(new GetMonitoringObserverSpecification(request.ElectionRoundId, request.MonitoringNgoId), ct);

        return TypedResults.Ok(new Response { MonitoringObservers = monitoringObservers });
    }
}
