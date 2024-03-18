using System.Net;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Create;

public class Endpoint : Endpoint<Request, Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, StatusCodeHttpResult>>
{
    private readonly IRepository<PollingStationAttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly ITimeProvider _timeProvider;

    public Endpoint(IRepository<PollingStationAttachmentAggregate> repository,
        IFileStorageService fileStorageService,
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
        _fileStorageService = fileStorageService;
    }

    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("attachments"));
        AllowFileUploads();
    }

    public override async Task<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, StatusCodeHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
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

        var uploadPath = $"elections/{electionRound.Id}/polling-stations/{pollingStation.Id}/attachments";
        
        var pollingStationAttachment = new PollingStationAttachmentAggregate(electionRound,
            monitoringObserver,
            req.Attachment.FileName,
            uploadPath,
            req.Attachment.ContentType,
            _timeProvider);

        var extension = req.Attachment.FileName.Split('.').Last();
        var fileNameWithExtension = $"{pollingStationAttachment.Id}.{extension}";

        var uploadResult = await _fileStorageService.UploadFileAsync(uploadPath,
            fileName: fileNameWithExtension,
            req.Attachment.OpenReadStream(),
            ct);

        if (uploadResult is UploadFileResult.Failed)
        {
            return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
        }

        await _repository.AddAsync(pollingStationAttachment, ct);

        //add tests
        var result = uploadResult as UploadFileResult.Ok;

        return TypedResults.Ok(new AttachmentModel
        {
            FileName = pollingStationAttachment.FileName,
            PresignedUrl = result!.Url,
            MimeType = pollingStationAttachment.MimeType,
            UrlValidityInSeconds = result.UrlValidityInSeconds,
            Id = pollingStationAttachment.Id
        });
    }
}
