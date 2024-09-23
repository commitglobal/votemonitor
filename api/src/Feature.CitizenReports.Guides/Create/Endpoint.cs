using System.Net;
using Authorization.Policies;
using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.CitizenReportGuideAggregate;

namespace Feature.CitizenReports.Guides.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<CitizenReportGuideAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<CitizenReportsGuideModel>, NotFound, StatusCodeHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports-guides");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports-guides"));
        AllowFileUploads();
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CitizenReportsGuideModel>, NotFound, StatusCodeHttpResult>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        CitizenReportGuideAggregate? observerGuide = null;
        CitizenReportsGuideModel? observerGuideModel = null;
        if (req.GuideType == CitizenReportGuideType.Document)
        {
            var uploadPath = $"elections/{req.ElectionRoundId}/citizen-reports-guides";

            observerGuide = CitizenReportGuideAggregate.NewDocumentGuide(req.ElectionRoundId,
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
            observerGuideModel = new CitizenReportsGuideModel
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

        if (req.GuideType == CitizenReportGuideType.Website)
        {
            observerGuide = CitizenReportGuideAggregate.NewWebsiteGuide(req.ElectionRoundId,
                req.Title,
                new Uri(req.WebsiteUrl!));

            observerGuideModel = new CitizenReportsGuideModel
            {
                Id = observerGuide.Id,
                Title = observerGuide.Title,
                WebsiteUrl = observerGuide.WebsiteUrl,
                GuideType = observerGuide.GuideType
            };
        }

        if (req.GuideType == CitizenReportGuideType.Text)
        {
            observerGuide = CitizenReportGuideAggregate.NewTextGuide(req.ElectionRoundId,
                req.Title,
                req.Text!);

            observerGuideModel = new CitizenReportsGuideModel
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