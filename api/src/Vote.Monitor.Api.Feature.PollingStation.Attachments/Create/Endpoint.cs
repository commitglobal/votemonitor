﻿using System.Net;
using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Create;

public class Endpoint : Endpoint<Request, Results<Ok<AttachmentModel>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<PollingStationAttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;

    public Endpoint(IAuthorizationService authorizationService,
        IRepository<PollingStationAttachmentAggregate> repository,
        IFileStorageService fileStorageService,
        IRepository<PollingStationAggregate> pollingStationRepository,
        IRepository<MonitoringObserver> monitoringObserverRepository)
    {
        _repository = repository;
        _pollingStationRepository = pollingStationRepository;
        _monitoringObserverRepository = monitoringObserverRepository;
        _authorizationService = authorizationService;
        _fileStorageService = fileStorageService;
    }

    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        AllowFileUploads();
        Summary(s => {
            s.Summary = "Uploads an attachment for a specific polling station";
        });
    }

    public override async Task<Results<Ok<AttachmentModel>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
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

        var uploadPath = $"elections/{req.ElectionRoundId}/polling-stations/{pollingStation.Id}/attachments";
        
        var pollingStationAttachment = new PollingStationAttachmentAggregate(req.ElectionRoundId,
            pollingStation,
            monitoringObserver,
            req.Attachment.FileName,
            uploadPath,
            req.Attachment.ContentType);
        
        var uploadResult = await _fileStorageService.UploadFileAsync(uploadPath,
            fileName: pollingStationAttachment.UploadedFileName,
            req.Attachment.OpenReadStream(),
            ct);

        if (uploadResult is UploadFileResult.Failed)
        {
            return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
        }

        await _repository.AddAsync(pollingStationAttachment, ct);

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
