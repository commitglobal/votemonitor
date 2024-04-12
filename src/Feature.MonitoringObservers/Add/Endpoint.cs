using Authorization.Policies;
using Feature.MonitoringObservers.Specifications;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Feature.MonitoringObservers.Add;

public class Endpoint(
    IRepository<ElectionRoundAggregate> repository,
    IRepository<MonitoringNgoAggregate> monitoringNgoRepository,
    IReadRepository<ObserverAggregate> observerRepository,
    IRepository<MonitoringObserver> monitoringObserverRepository)
    : Endpoint<Request, Results<Ok<Response>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers");
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Adds a monitoring observer to selected election round and ngo";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetMonitoringObserverForElectionRound(req.ElectionRoundId, req.ObserverId);
        var isAlreadyMonitoring = await monitoringObserverRepository.AnyAsync(specification,ct);
        if (isAlreadyMonitoring)
        {
            AddError(x => x.ObserverId, "Observer is already monitoring for another ngo.");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var electionRound = await repository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
        }

        var monitoringNgo = await monitoringNgoRepository.SingleOrDefaultAsync(new GetMonitoringNgoWithObserversSpecification(req.ElectionRoundId, req.MonitoringNgoId), ct);
        if (monitoringNgo is null)
        {
            return TypedResults.NotFound("Monitoring NGO not found");
        }

        if (monitoringNgo.Ngo.Status == NgoStatus.Deactivated || monitoringNgo.Status == MonitoringNgoStatus.Suspended)
        {
            AddError(x => x.MonitoringNgoId, "Only active monitoring NGOs can add monitoring observers");
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

        var monitoringObserver = monitoringNgo.AddMonitoringObserver(observer);
        if (monitoringObserver is null)
        {
            AddError(x => x.ObserverId, "Observer is already registered as monitoring for this monitoring NGO.");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        await monitoringObserverRepository.AddAsync(monitoringObserver, ct);

        return TypedResults.Ok(new Response
        {
            Id = monitoringObserver.Id,
            InviterNgoId = monitoringObserver.MonitoringNgoId,
            ObserverId = observer.Id
        });
    }
}
