using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Create;

public class Endpoint : Endpoint<Request, Results<Ok<NoteModel>, NotFound, BadRequest<ProblemDetails>>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<PollingStationNoteAggregate> _repository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;

    public Endpoint(IAuthorizationService authorizationService,
        IRepository<PollingStationNoteAggregate> repository,
        IRepository<PollingStationAggregate> pollingStationRepository,
        IRepository<MonitoringObserver> monitoringObserverRepository)
    {
        _repository = repository;
        _pollingStationRepository = pollingStationRepository;
        _monitoringObserverRepository = monitoringObserverRepository;
        _authorizationService = authorizationService;
    }
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s =>
        {
            s.Summary = "Creates a note for a polling station";
        });
    }

    public override async Task<Results<Ok<NoteModel>, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
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

        var pollingStationNote = new PollingStationNoteAggregate(req.ElectionRoundId,
            pollingStation,
            monitoringObserver,
            req.Text);

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
