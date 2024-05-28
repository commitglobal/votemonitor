using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Feature.ObserverGuide.List;

public class Endpoint(IAuthorizationService authorizationService,
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
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        List<ObserverGuideAggregate> observerGuides;
        if (currentUserRoleProvider.IsNgoAdmin())
        {
            var ngoId = currentUserProvider.GetNgoId()!.Value;
            // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
            observerGuides = await context
                .ObserversGuides
                .Where(x => x.MonitoringNgo.NgoId == ngoId
                            && x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                                                           && !x.IsDeleted)
                .ToListAsync(ct);
        }
        else if (currentUserRoleProvider.IsObserver())
        {
            var observerId = currentUserProvider.GetUserId()!.Value;

            // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
            observerGuides = await context
                .ObserversGuides
                .Where(x => x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                            && x.MonitoringNgo.MonitoringObservers.Any(mo => mo.ObserverId == observerId)
                            && !x.IsDeleted)
                .ToListAsync(ct);
        }
        else
        {
            return TypedResults.NotFound();
        }

        var tasks = observerGuides
            .Where(x => x.GuideType == ObserverGuideType.Document)
            .Select(async guide =>
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
                    MimeType = guide.MimeType,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                    GuideType = ObserverGuideType.Document,
                    CreatedOn = guide.CreatedOn,
                    CreatedBy = "-" // TODO: use dapper to do the joinery
                };
            });

        var documentObserverGuides = await Task.WhenAll(tasks);

        var websiteObserverGuides = observerGuides
            // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
            .Where(x => x.GuideType == ObserverGuideType.Website)
            .Select(guide => new ObserverGuideModel
            {
                Id = guide.Id,
                Title = guide.Title,
                WebsiteUrl = guide.WebsiteUrl,
                GuideType = ObserverGuideType.Website,
                CreatedOn = guide.CreatedOn,
                CreatedBy = "-" // TODO: use dapper to do the joinery
            });

        var guides = documentObserverGuides
            .Concat(websiteObserverGuides)
            .OrderBy(x => x.Title);

        return TypedResults.Ok(new Response { Guides = guides.ToList() });
    }
}
