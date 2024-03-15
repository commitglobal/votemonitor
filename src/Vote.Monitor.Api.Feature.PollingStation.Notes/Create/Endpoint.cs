using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Create;

public class Endpoint : Endpoint<Request, Results<Ok<NoteModel>, BadRequest<ProblemDetails>>>
{
    private readonly IRepository<PollingStationNoteAggregate> _repository;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly ITimeProvider _timeProvider;

    public Endpoint(IRepository<PollingStationNoteAggregate> repository,
        IRepository<ElectionRound> electionRoundRepository,
        IRepository<PollingStationAggregate> pollingStationRepository,
        IRepository<MonitoringObserver> monitoringObserverRepository,
        ITimeProvider timeProvider)
    {
        _repository = repository;
        _electionRoundRepository = electionRoundRepository;
        _pollingStationRepository = pollingStationRepository;
        _monitoringObserverRepository = monitoringObserverRepository;
        _timeProvider = timeProvider;
    }
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("notes"));
    }

    public override async Task<Results<Ok<NoteModel>, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRoundSpecification = new GetElectionRoundSpecification(req.ElectionRoundId);
        var electionRound = await _electionRoundRepository.FirstOrDefaultAsync(electionRoundSpecification, ct);

        if (electionRound == null)
        {
            AddError(r => r.ElectionRoundId, "Election round not found");
            return TypedResults.BadRequest(new ProblemDetails(ValidationFailures));
        }

        var pollingStationSpecification = new GetPollingStationSpecification(req.PollingStationId);
        var pollingStation = await _pollingStationRepository.FirstOrDefaultAsync(pollingStationSpecification, ct);

        if (pollingStation == null)
        {
            AddError(r => r.PollingStationId, "Polling station not found");
            return TypedResults.BadRequest(new ProblemDetails(ValidationFailures));
        }

        var monitoringObserverSpecification = new GetMonitoringObserverSpecification(req.ObserverId);
        var monitoringObserver = await _monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        if (monitoringObserver == null)
        {
            AddError(r => r.ObserverId, "Observer not found");
            return TypedResults.BadRequest(new ProblemDetails(ValidationFailures));
        }

        var pollingStationNote = new PollingStationNoteAggregate(electionRound,
            pollingStation,
            monitoringObserver,
            req.Text,
            _timeProvider);

        await _repository.AddAsync(pollingStationNote, ct);

        return TypedResults.Ok(new NoteModel
        {
            Id = pollingStationNote.Id,
            Text = pollingStationNote.Text,
            CreatedAt = pollingStationNote.CreatedOn,
            UpdatedAt = null
        });
    }
}
