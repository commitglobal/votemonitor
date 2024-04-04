using Ardalis.Specification;
using Authorization.Policies.Requirements;
using Feature.ObserverGuide.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;

namespace Feature.ObserverGuide.List;

public class Endpoint(IAuthorizationService authorizationService,
    ICurrentUserProvider currentUserProvider,
    IReadRepository<ObserverGuideAggregate> repository,
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
        
        Specification<ObserverGuideAggregate> specification = null!;
        if (currentUserProvider.IsObserver())
        {
            specification = new GetObserverGuidesSpecification(currentUserProvider.GetUserId());
        }
        else if (currentUserProvider.IsNgoAdmin())
        { 
            specification = new GetObserverGuidesForNgoAdminSpecification(currentUserProvider.GetNgoId());
        }

        var observerGuides = await repository.ListAsync(specification, ct);

        var observerGuideModels = observerGuides
            .Select(async observerGuide =>
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    observerGuide.FilePath,
                    observerGuide.UploadedFileName,
                    ct);

                return new ObserverGuideModel
                {
                    Title = observerGuide.Title,
                    FileName = observerGuide.FileName,
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    MimeType = observerGuide.MimeType,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                    Id = observerGuide.Id
                };
            })
            .Select(t => t.Result)
            .ToList();

        return TypedResults.Ok(new Response { Guides = observerGuideModels });
    }
}
