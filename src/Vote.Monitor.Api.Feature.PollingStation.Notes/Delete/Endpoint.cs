using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Delete;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<PollingStationNoteAggregate> _repository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;

    public Endpoint(IAuthorizationService authorizationService, 
        IRepository<PollingStationNoteAggregate> repository, 
        IRepository<PollingStationAggregate> pollingStationRepository)
    {
        _authorizationService = authorizationService;
        _repository = repository;
        _pollingStationRepository = pollingStationRepository;
    }

    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("notes", "mobile"));
        Summary(s => {
            s.Summary = "Deletes a note for a polling station";
        });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
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

        await _repository.DeleteAsync(pollingStationNote, ct);

        return TypedResults.NoContent();
    }
}
