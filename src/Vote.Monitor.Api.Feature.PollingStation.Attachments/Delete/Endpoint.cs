using Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Delete;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    private readonly IRepository<PollingStationAttachmentAggregate> _repository;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;

    public Endpoint(IRepository<PollingStationAttachmentAggregate> repository,
        IRepository<ElectionRound> electionRoundRepository,
        IRepository<PollingStationAggregate> pollingStationRepository)
    {
        _repository = repository;
        _electionRoundRepository = electionRoundRepository;
        _pollingStationRepository = pollingStationRepository;
    }

    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        Summary(s => {
            s.Summary = "Deletes an attachment";
        });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await _electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);

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

        var specification = new GetPollingStationAttachmentSpecification(req.ElectionRoundId,
            req.PollingStationId,
            req.ObserverId,
            req.Id);
        var pollingStationAttachment = await _repository.FirstOrDefaultAsync(specification, ct);
        if (pollingStationAttachment is null)
        {
            return TypedResults.NotFound();
        }

        pollingStationAttachment.Delete();

        await _repository.UpdateAsync(pollingStationAttachment, ct);
        
        return TypedResults.NoContent();
    }
}
