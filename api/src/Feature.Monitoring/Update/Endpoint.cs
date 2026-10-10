using Authorization.Policies;
using Feature.Monitoring.Specifications;

namespace Feature.Monitoring.Update;

public class Endpoint(IRepository<MonitoringNgoAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/monitoring-ngos/{ngoId}");
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var monitoringNgo =
            await repository.FirstOrDefaultAsync(new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId),
                ct);

        if (monitoringNgo is null)
        {
            return TypedResults.NotFound();
        }

        monitoringNgo.UpdateAllowMultipleFormSubmission(req.AllowMultipleFormSubmission);
        await repository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }
}
