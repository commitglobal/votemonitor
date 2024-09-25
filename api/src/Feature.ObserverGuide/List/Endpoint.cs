using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Feature.ObserverGuide.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserProvider currentUserProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    VoteMonitorContext context,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/observer-guide");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new MonitoringNgoAdminOrObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var isNgoAdmin = currentUserRoleProvider.IsNgoAdmin();
        var isObserver = currentUserRoleProvider.IsObserver();


        var observerId = currentUserProvider.GetUserId();
        var ngoId = currentUserProvider.GetNgoId();

        // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
        var guides = await context
            .ObserversGuides
            .Where(x => x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId && !x.IsDeleted)
            .Where(x => isObserver || x.MonitoringNgo.NgoId == ngoId)
            .Where(x => isNgoAdmin || x.MonitoringNgo.MonitoringObservers.Any(mo => mo.ObserverId == observerId))
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
                CreatedBy =  user.FirstName + " " + user.LastName
            })
            .AsNoTracking()
            .ToListAsync(ct);

        var tasks = guides
            .Select(async guide =>
            {
                if (guide.GuideType == ObserverGuideType.Document)
                {
                    var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                        guide.FilePath!,
                        guide.UploadedFileName!);

                    return new ObserverGuideModel
                    {
                        Id = guide.Id,
                        Title = guide.Title,
                        FileName = guide.FileName,
                        PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                        UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                        MimeType = guide.MimeType,
                        GuideType = guide.GuideType,
                        CreatedOn = guide.CreatedOn,
                        CreatedBy = guide.CreatedBy
                    };
                }

                return new ObserverGuideModel
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
            });

        var mappedGuides = await Task.WhenAll(tasks);

        return TypedResults.Ok(new Response { Guides = mappedGuides.ToList() });
    }
}