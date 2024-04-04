using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.ObserverGuide.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;

namespace Feature.ObserverGuide.Get;

public class Endpoint(IAuthorizationService authorizationService,
    ICurrentUserProvider currentUserProvider, 
    IReadRepository<ObserverGuideAggregate> repository,
    IFileStorageService fileStorageService) 
    : Endpoint<Request, Results<Ok<ObserverGuideModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/observer-guide/{id}");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
    }

    public override async Task<Results<Ok<ObserverGuideModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        ObserverGuideAggregate? observerGuide = null;

        if (currentUserProvider.IsObserver())
        {
            var specification = new GetObserverGuideSpecification(currentUserProvider.GetUserId(), req.Id);
            observerGuide = await repository.FirstOrDefaultAsync(specification, ct);
        }
        else if(currentUserProvider.IsNgoAdmin())
        {
            var specification = new GetObserverGuideForNgoAdminSpecification(currentUserProvider.GetNgoId(), req.Id);
            observerGuide = await repository.FirstOrDefaultAsync(specification, ct);
        }

        if (observerGuide == null)
        {
            return TypedResults.NotFound();
        }

        var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
            observerGuide.FilePath,
            observerGuide.UploadedFileName,
            ct);

        return TypedResults.Ok(new ObserverGuideModel
        {
            Title = observerGuide.Title,
            FileName = observerGuide.FileName,
            PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
            MimeType = observerGuide.MimeType,
            UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
            Id = observerGuide.Id
        });
    }
}
