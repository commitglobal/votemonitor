using System.Net;
using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.ObserverGuide.Model;
using Feature.ObserverGuide.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Feature.ObserverGuide.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<ObserverGuideAggregate> repository,
    IReadRepository<MonitoringNgo> monitoringNgoRepository,
    IFileStorageService fileStorageService,
    IHtmlStringSanitizer htmlStringSanitizer)
    : Endpoint<Request, Results<Ok<ObserverGuideModel>, NotFound, StatusCodeHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/observer-guide");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
        AllowFileUploads();
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<ObserverGuideModel>, NotFound, StatusCodeHttpResult>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId);
        var monitoringNgo = await monitoringNgoRepository.FirstOrDefaultAsync(specification, ct);
        if (monitoringNgo == null)
        {
            return TypedResults.NotFound();
        }

        ObserverGuideAggregate? observerGuide = null;
        ObserverGuideModel? observerGuideModel = null;
        if (req.GuideType == ObserverGuideType.Document)
        {
            var uploadPath = $"elections/{req.ElectionRoundId}/ngo/{monitoringNgo.NgoId}/observer-guides";

            observerGuide = ObserverGuideAggregate.NewDocumentGuide(monitoringNgo,
                req.Title,
                req.Attachment!.FileName,
                uploadPath,
                req.Attachment.ContentType);

            var uploadResult = await fileStorageService.UploadFileAsync(uploadPath,
                fileName: observerGuide.UploadedFileName!,
                req.Attachment.OpenReadStream(),
                ct);

            if (uploadResult is UploadFileResult.Failed)
            {
                return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
            }

            var result = uploadResult as UploadFileResult.Ok;
            observerGuideModel = new ObserverGuideModel
            {
                Title = observerGuide.Title,
                FileName = observerGuide.FileName!,
                PresignedUrl = result!.Url,
                MimeType = observerGuide.MimeType!,
                UrlValidityInSeconds = result.UrlValidityInSeconds,
                Id = observerGuide.Id,
                GuideType = observerGuide.GuideType
            };
        }

        if (req.GuideType == ObserverGuideType.Website)
        {
            observerGuide = ObserverGuideAggregate.NewWebsiteGuide(monitoringNgo,
                req.Title,
                new Uri(req.WebsiteUrl!));

            observerGuideModel = new ObserverGuideModel
            {
                Id = observerGuide.Id,
                Title = observerGuide.Title,
                WebsiteUrl = observerGuide.WebsiteUrl,
                GuideType = observerGuide.GuideType
            };
        }

        if (req.GuideType == ObserverGuideType.Text)
        {
            observerGuide = ObserverGuideAggregate.NewTextGuide(monitoringNgo,
                req.Title,
                htmlStringSanitizer.Sanitize(req.Text!));

            observerGuideModel = new ObserverGuideModel
            {
                Id = observerGuide.Id,
                Title = observerGuide.Title,
                Text = observerGuide.Text,
                GuideType = observerGuide.GuideType
            };
        }

        await repository.AddAsync(observerGuide!, ct);
        return TypedResults.Ok(observerGuideModel!);
    }
}