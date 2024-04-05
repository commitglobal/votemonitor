using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.List;

public class Endpoint : Endpoint<Request, Results<Ok<List<NoteModel>> , NotFound, BadRequest<ProblemDetails>>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<PollingStationNoteAggregate> _repository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;

    public Endpoint(IAuthorizationService authorizationService, 
        IReadRepository<PollingStationNoteAggregate> repository,
        IRepository<PollingStationAggregate> pollingStationRepository)
    {
        _authorizationService = authorizationService;
        _repository = repository;
        _pollingStationRepository = pollingStationRepository;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s => {
            s.Summary = "Lists notes for a polling station";
        });
    }

    public override async Task<Results<Ok<List<NoteModel>>, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
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

        var specification = new GetPollingStationNotesSpecification(req.ElectionRoundId, req.PollingStationId, req.ObserverId);
        var pollingStationNotes = await _repository.ListAsync(specification, ct);

        return TypedResults.Ok(pollingStationNotes
            .Select(m => new NoteModel
            {
                Id = m.Id,
                Text = m.Text,
                CreatedAt = m.CreatedOn,
                UpdatedAt = m.LastModifiedOn
            })
            .ToList()
        );
    }
}
