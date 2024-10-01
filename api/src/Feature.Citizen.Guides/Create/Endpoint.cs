using System.Net;
using Authorization.Policies;
using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;

namespace Feature.Citizen.Guides.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<CitizenGuideAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<CitizenGuideModel>, NotFound, StatusCodeHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-guides");
        DontAutoTag();
        Options(x => x.WithTags("citizen-guides"));
        AllowFileUploads();
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CitizenGuideModel>, NotFound, StatusCodeHttpResult>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        CitizenGuideAggregate? citizenGuide = null;
        CitizenGuideModel? citizenGuideModel = null;
        if (req.GuideType == CitizenGuideType.Document)
        {
            var uploadPath = $"elections/{req.ElectionRoundId}/citizen-guides";

            citizenGuide = CitizenGuide.NewDocumentGuide(req.ElectionRoundId,
                req.Title,
                req.Attachment!.FileName,
                uploadPath,
                req.Attachment.ContentType);

            var uploadResult = await fileStorageService.UploadFileAsync(uploadPath,
                fileName: citizenGuide.UploadedFileName!,
                req.Attachment.OpenReadStream(),
                ct);

            if (uploadResult is UploadFileResult.Failed)
            {
                return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
            }

            var result = uploadResult as UploadFileResult.Ok;
            citizenGuideModel = new CitizenGuideModel
            {
                Title = citizenGuide.Title,
                FileName = citizenGuide.FileName!,
                PresignedUrl = result!.Url,
                MimeType = citizenGuide.MimeType!,
                UrlValidityInSeconds = result.UrlValidityInSeconds,
                Id = citizenGuide.Id,
                GuideType = citizenGuide.GuideType
            };
        }

        if (req.GuideType == CitizenGuideType.Website)
        {
            citizenGuide = CitizenGuide.NewWebsiteGuide(req.ElectionRoundId,
                req.Title,
                new Uri(req.WebsiteUrl!));

            citizenGuideModel = new CitizenGuideModel
            {
                Id = citizenGuide.Id,
                Title = citizenGuide.Title,
                WebsiteUrl = citizenGuide.WebsiteUrl,
                GuideType = citizenGuide.GuideType
            };
        }

        if (req.GuideType == CitizenGuideType.Text)
        {
            citizenGuide = CitizenGuide.NewTextGuide(req.ElectionRoundId,
                req.Title,
                req.Text!);

            citizenGuideModel = new CitizenGuideModel
            {
                Id = citizenGuide.Id,
                Title = citizenGuide.Title,
                Text = citizenGuide.Text,
                GuideType = citizenGuide.GuideType
            };
        }

        await repository.AddAsync(citizenGuide!, ct);
        return TypedResults.Ok(citizenGuideModel!);
    }
}