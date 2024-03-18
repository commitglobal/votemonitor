using Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Get;

public class Endpoint : Endpoint<Request, Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, NotFound>>
{
    private readonly IReadRepository<PollingStationAttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;

    public Endpoint(IReadRepository<PollingStationAttachmentAggregate> repository,
        IFileStorageService fileStorageService,
        IRepository<ElectionRound> electionRoundRepository,
        IRepository<PollingStationAggregate> pollingStationRepository)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        _electionRoundRepository = electionRoundRepository;
        _pollingStationRepository = pollingStationRepository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("attachments"));
    }

    public override async Task<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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

        var specification = new GetPollingStationAttachmentSpecification(req.ElectionRoundId,
            req.PollingStationId,
            req.ObserverId,
            req.Id);
        var pollingStationAttachment = await _repository.FirstOrDefaultAsync(specification, ct);
        if (pollingStationAttachment is null)
        {
            return TypedResults.NotFound();
        }

        var presignedUrl = await _fileStorageService.GetPresignedUrlAsync(pollingStationAttachment.FilePath, 
            pollingStationAttachment.FileName,
            ct);

        return TypedResults.Ok(new AttachmentModel
        {
            FileName = pollingStationAttachment.FileName,
            PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
            MimeType = pollingStationAttachment.MimeType,
            UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
            Id = pollingStationAttachment.Id
        });
    }
}
