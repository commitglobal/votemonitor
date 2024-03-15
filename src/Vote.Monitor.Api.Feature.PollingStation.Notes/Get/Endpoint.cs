using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Get;

public class Endpoint : Endpoint<Request, Results<Ok<NoteModel>, BadRequest<ProblemDetails>, NotFound>>
{
    private readonly IReadRepository<PollingStationNoteAggregate> _repository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<ElectionRound> _electionRoundRepository;

    public Endpoint(IReadRepository<PollingStationNoteAggregate> repository,
        IRepository<PollingStationAggregate> pollingStationRepository,
        IRepository<ElectionRound> electionRoundRepository)
    {
        _repository = repository;
        _pollingStationRepository = pollingStationRepository;
        _electionRoundRepository = electionRoundRepository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("notes"));
    }

    public override async Task<Results<Ok<NoteModel>, BadRequest<ProblemDetails>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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

        var pollingStationNote = await _repository.GetByIdAsync(req.Id, ct);
        if (pollingStationNote is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new NoteModel
        {
            Id = pollingStationNote.Id,
            Text = pollingStationNote.Text,
            CreatedAt = pollingStationNote.CreatedOn,
            UpdatedAt = pollingStationNote.Timestamp
        });
    }
}
