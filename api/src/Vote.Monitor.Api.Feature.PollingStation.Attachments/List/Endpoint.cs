using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.List;

public class Endpoint : Endpoint<Request, Results<Ok<List<AttachmentModel>>, NotFound, BadRequest<ProblemDetails>>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<PollingStationAttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;

    public Endpoint(IAuthorizationService authorizationService,
        IReadRepository<PollingStationAttachmentAggregate> repository,
        IFileStorageService fileStorageService,
        IRepository<PollingStationAggregate> pollingStationRepository)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        _pollingStationRepository = pollingStationRepository;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets all attachments an observer has uploaded for a polling station";
            s.Description = "Gets all attachments with freshly generated presigned urls";
        });
    }

    public override async Task<Results<Ok<List<AttachmentModel>>, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
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

        var specification = new GetObserverPollingStationAttachmentsSpecification(req.ElectionRoundId, req.PollingStationId, req.ObserverId);
        var attachments = await _repository.ListAsync(specification, ct);

        var attachmentModels = attachments
            .Select(async pollingStationAttachment =>
            {
                var presignedUrl = await _fileStorageService.GetPresignedUrlAsync(
                    pollingStationAttachment.FilePath,
                    pollingStationAttachment.UploadedFileName,
                    ct);

                return new AttachmentModel
                {
                    FileName = pollingStationAttachment.FileName,
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    MimeType = pollingStationAttachment.MimeType,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                    Id = pollingStationAttachment.Id
                };
            })
            .Select(t => t.Result)
            .ToList();

        return TypedResults.Ok(attachmentModels);
    }
}
