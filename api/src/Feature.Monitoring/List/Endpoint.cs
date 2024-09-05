using Authorization.Policies;
using Feature.Monitoring.Specifications;

namespace Feature.Monitoring.List;
public class Endpoint(IReadRepository<MonitoringNgoAggregate> repository) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-ngos");
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var monitoringNgos = await repository.ListAsync(new GetMonitoringNgoSpecification(request.ElectionRoundId), ct);

        return TypedResults.Ok(new Response { MonitoringNgos = monitoringNgos });
    }
}
