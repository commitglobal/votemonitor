using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Update;

public class Endpoint : Endpoint<Request, Results<Ok<NoteModel>, NotFound, BadRequest<ProblemDetails>>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<PollingStationNoteAggregate> _repository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;

    public Endpoint(IAuthorizationService authorizationService, 
        IRepository<PollingStationNoteAggregate> repository,
        IRepository<PollingStationAggregate> pollingStationRepository)
    {
        _repository = repository;
        _pollingStationRepository = pollingStationRepository;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s =>
        {
            s.Summary = "Updates a note for a polling station";
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

        var specification = new GetPollingStationNoteSpecification(req.ElectionRoundId,
            req.PollingStationId,
            req.ObserverId,
            req.Id);
        var pollingStationNote = await _repository.FirstOrDefaultAsync(specification, ct);

        if (pollingStationNote == null)
        {
            return TypedResults.NotFound();
        }

        pollingStationNote.UpdateText(req.Text);
        await _repository.UpdateAsync(pollingStationNote, ct);

        return TypedResults.Ok(new NoteModel
        {
            Id = pollingStationNote.Id,
            Text = pollingStationNote.Text,
            CreatedAt = pollingStationNote.CreatedOn,
            UpdatedAt = pollingStationNote.LastModifiedOn
        });
    }
}
