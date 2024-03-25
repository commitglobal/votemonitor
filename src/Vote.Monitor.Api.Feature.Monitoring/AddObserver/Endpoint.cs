using Vote.Monitor.Api.Feature.Monitoring.Specifications;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.AddObserver;

public class Endpoint : Endpoint<Request, Results<Ok<MonitoringObserverModel>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>>
{
    private readonly IRepository<ElectionRoundAggregate> _repository;
    private readonly IRepository<MonitoringNgo> _monitoringNgoRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly IReadRepository<Observer> _observerRepository;
    private readonly ITimeProvider _timeProvider;

    public Endpoint(IRepository<ElectionRoundAggregate> repository,
        IRepository<MonitoringNgoAggregate> monitoringNgoRepository,
        IReadRepository<ObserverAggregate> observerRepository,
        IRepository<MonitoringObserver> monitoringObserverRepository, 
        ITimeProvider timeProvider)
    {
        _repository = repository;
        _monitoringNgoRepository = monitoringNgoRepository;
        _observerRepository = observerRepository;
        _monitoringObserverRepository = monitoringObserverRepository;
        _timeProvider = timeProvider;
    }

    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers");
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
    }

    public override async Task<Results<Ok<MonitoringObserverModel>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await _repository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
        }

        var monitoringNgo = await _monitoringNgoRepository.SingleOrDefaultAsync(new GetMonitoringNgoSpecification(req.ElectionRoundId, req.MonitoringNgoId), ct);
        if (monitoringNgo is null)
        {
            return TypedResults.NotFound("Monitoring NGO not found");
        }

        if (monitoringNgo.Ngo.Status == NgoStatus.Deactivated || monitoringNgo.Status == MonitoringNgoStatus.Suspended)
        {
            AddError(x => x.MonitoringNgoId, "Only active monitoring NGOs can add monitoring observers");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var observer = await _observerRepository.GetByIdAsync(req.ObserverId, ct);
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

        await _monitoringObserverRepository.AddAsync(monitoringObserver, ct);

        return TypedResults.Ok(new MonitoringObserverModel
        {
            Id = monitoringObserver.Id,
            InviterNgoId = monitoringObserver.InviterNgoId,
            Status = monitoringObserver.Status
        });
    }
}
