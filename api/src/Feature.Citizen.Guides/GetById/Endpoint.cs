using Authorization.Policies;
using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;

namespace Feature.Citizen.Guides.GetById;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<CitizenGuideModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-guides/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-guides"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CitizenGuideModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
        var guide = await context
            .CitizenGuides
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && !x.IsDeleted && x.Id == req.Id)
            .OrderByDescending(x => x.CreatedOn)
            .Join(context.NgoAdmins, guide => guide.LastModifiedBy == Guid.Empty ? guide.CreatedBy : guide.LastModifiedBy, user => user.Id, (guide, ngoAdmin) => new
            {
                guide.Id,
                guide.Title,
                guide.FileName,
                guide.UploadedFileName,
                guide.MimeType,
                guide.GuideType,
                guide.CreatedOn,
                guide.FilePath,
                guide.Text,
                guide.WebsiteUrl,
                UserId = ngoAdmin.ApplicationUserId
            })
            .Join(context.Users, x => x.UserId, user => user.Id, (guide, user) => new
            {
                guide.Id,
                guide.Title,
                guide.FileName,
                guide.UploadedFileName,
                guide.MimeType,
                guide.GuideType,
                guide.CreatedOn,
                guide.FilePath,
                guide.Text,
                guide.WebsiteUrl,
                CreatedBy = user.FirstName + " " + user.LastName
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (guide is null)
        {
            return TypedResults.NotFound();
        }

        var citizenGuideModel = new CitizenGuideModel
        {
            Id = guide.Id,
            Title = guide.Title,
            FileName = guide.FileName,
            MimeType = guide.MimeType,
            GuideType = guide.GuideType,
            CreatedOn = guide.CreatedOn,
            Text = guide.Text,
            WebsiteUrl = guide.WebsiteUrl,
            CreatedBy = guide.CreatedBy
        };

        if (guide.GuideType == CitizenGuideType.Document)
        {
            var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                guide.FilePath!,
                guide.UploadedFileName!);

            return TypedResults.Ok(citizenGuideModel with
            {
                PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0
            });
        }

        return TypedResults.Ok(citizenGuideModel);
    }
}