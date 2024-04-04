using System.Net;
using Authorization.Policies.Requirements;
using Feature.ObserverGuide.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.ObserverGuide.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<ObserverGuideAggregate> repository,
    ICurrentUserProvider currentUserProvider,
    IReadRepository<MonitoringNgo> monitoringNgoRepository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<ObserverGuideModel>, NotFound, StatusCodeHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/observer-guide");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
        AllowFileUploads();
    }

    public override async Task<Results<Ok<ObserverGuideModel>, NotFound, StatusCodeHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetMonitoringNgoSpecification(currentUserProvider.GetNgoId(), req.ElectionRoundId);
        var monitoringNgo = await monitoringNgoRepository.FirstOrDefaultAsync(specification, ct);
        if (monitoringNgo == null)
        {
            return TypedResults.NotFound();
        }

        var uploadPath = $"elections/{req.ElectionRoundId}/ngo/{monitoringNgo.NgoId}/observer-guides";

        var observerGuide = new ObserverGuideAggregate(monitoringNgo,
            req.Title,
            req.Attachment.FileName,
            uploadPath,
            req.Attachment.ContentType);

        var uploadResult = await fileStorageService.UploadFileAsync(uploadPath,
            fileName: observerGuide.UploadedFileName,
            req.Attachment.OpenReadStream(),
            ct);

        if (uploadResult is UploadFileResult.Failed)
        {
            return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
        }

        await repository.AddAsync(observerGuide, ct);

        var result = uploadResult as UploadFileResult.Ok;

        return TypedResults.Ok(new ObserverGuideModel
        {
            Title = observerGuide.Title,
            FileName = observerGuide.FileName,
            PresignedUrl = result!.Url,
            MimeType = observerGuide.MimeType,
            UrlValidityInSeconds = result.UrlValidityInSeconds,
            Id = observerGuide.Id
        });
    }
}
