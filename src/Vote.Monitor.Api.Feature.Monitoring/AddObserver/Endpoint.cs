using Vote.Monitor.Api.Feature.Monitoring.Specifications;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.AddObserver;

public class Endpoint(IRepository<ElectionRoundAggregate> repository,
    IRepository<MonitoringNgoAggregate> monitoringNgoRepository,
    IReadRepository<ObserverAggregate> observerRepository,
    ITimeProvider timeProvider) : Endpoint<Request, Results<NoContent, NotFound<string>, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngos/{ngoId}/monitoring-observers");
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
    }

    public override async Task<Results<NoContent, NotFound<string>, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
        }

        var monitoringNgo = await monitoringNgoRepository.SingleOrDefaultAsync(new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        if (monitoringNgo is null)
        {
            return TypedResults.NotFound("NGO not found");
        }

        if (monitoringNgo.Ngo.Status == NgoStatus.Deactivated || monitoringNgo.Status == MonitoringNgoStatus.Suspended)
        {
            AddError(x => x.NgoId, "Only active ngos can add monitoring observers");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var observer = await observerRepository.GetByIdAsync(req.ObserverId, ct);
        if (observer is null)
        {
            return TypedResults.NotFound("Observer not found");
        }

        if (observer.Status == UserStatus.Deactivated)
        {
            AddError(x => x.ObserverId, "Only active observers can monitor elections");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        monitoringNgo.AddMonitoringObserver(observer, timeProvider);

        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);
        return TypedResults.NoContent();
    }
}
