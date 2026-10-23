using Authorization.Policies.Requirements;
using Dapper;
using Feature.ObserverGuide.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Feature.ObserverGuide.List;

public class Endpoint(
    INpgsqlConnectionFactory dbConnectionFactory,
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

        var isNgoAdmin = currentUserRoleProvider.IsNgoAdmin() || currentUserRoleProvider.IsNgoStaff();
        var isObserver = currentUserRoleProvider.IsObserver();

        if (isObserver)
        {
            return await ListObserverGuidesAsObserverAsync(req.ElectionRoundId, ct);
        }

        if (isNgoAdmin)
        {
            return await ListObserverGuidesAsNgoAdminAsync(req.ElectionRoundId, ct);
        }

        return TypedResults.NotFound();
    }

    private async Task<Results<Ok<Response>, NotFound>> ListObserverGuidesAsNgoAdminAsync(Guid electionRoundId,
        CancellationToken ct)
    {
        var ngoId = currentUserProvider.GetNgoId();
        if (ngoId == null || ngoId.Value == Guid.Empty)
        {
            ThrowError("NGO id is empty");
        }

        var sql =
            """
            SELECT * FROM "GetNgoObserverGuides"(@electionRoundId, @ngoId, NULL)
            """;

        var queryArgs = new { electionRoundId, ngoId, };

        List<ObserverGuideModel> ngoGuides;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            ngoGuides = (await dbConnection.QueryAsync<ObserverGuideModel>(sql, queryArgs)).ToList();
        }

        for (var index = 0; index < ngoGuides.Count; index++)
        {
            var guide = ngoGuides[index];
            if (guide.GuideType == ObserverGuideType.Document)
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    guide.FilePath!,
                    guide.UploadedFileName!);

                ngoGuides[index] = guide with
                {
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                };
            }
        }

        return TypedResults.Ok(new Response { Guides = ngoGuides });
    }

    private async Task<Results<Ok<Response>, NotFound>> ListObserverGuidesAsObserverAsync(Guid electionRoundId,
        CancellationToken ct)
    {
        var observerId = currentUserProvider.GetUserId();
        if (observerId == null || observerId.Value == Guid.Empty)
        {
            ThrowError("Observer id is empty");
        }

        var ngo = await context.MonitoringNgos.AsNoTracking()
            .Where(x=>x.ElectionRoundId == electionRoundId)
            .Where(x => x.MonitoringObservers.Any(mo => mo.ObserverId == observerId))
            .Select(x => new { x.NgoId })
            .FirstOrDefaultAsync(ct);

        if (ngo == null)
        {
            ThrowError("Ngo id is empty");
        }
        
        var sql =
            """
            SELECT * FROM "GetObserverGuidesForObserver"(@electionRoundId, @ngoId)
            """;

        var queryArgs = new { electionRoundId, ngoId = ngo.NgoId };

        List<ObserverGuideModel> observerGuides;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            observerGuides = (await dbConnection.QueryAsync<ObserverGuideModel>(sql, queryArgs)).ToList();
        }

        for (var index = 0; index < observerGuides.Count; index++)
        {
            var guide = observerGuides[index];
            if (guide.GuideType == ObserverGuideType.Document)
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    guide.FilePath!,
                    guide.UploadedFileName!);

                observerGuides[index] = guide with
                {
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                };
            }
        }

        return TypedResults.Ok(new Response { Guides = observerGuides });
    }
}
