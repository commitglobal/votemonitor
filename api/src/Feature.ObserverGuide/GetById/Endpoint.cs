using Authorization.Policies;
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

namespace Feature.ObserverGuide.GetById;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    ICurrentUserProvider currentUserProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    IFileStorageService fileStorageService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<ObserverGuideModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/observer-guide/{id}");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<ObserverGuideModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
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
            return await ListObserverGuidesAsObserverAsync(req.ElectionRoundId, req.Id, ct);
        }

        if (isNgoAdmin)
        {
            return await ListObserverGuidesAsNgoAdminAsync(req.ElectionRoundId, req.Id, ct);
        }

        return TypedResults.NotFound();
    }

    private async Task<Results<Ok<ObserverGuideModel>, NotFound>> ListObserverGuidesAsNgoAdminAsync(
        Guid electionRoundId,
        Guid guideId,
        CancellationToken ct)
    {
        var ngoId = currentUserProvider.GetNgoId();
        if (ngoId == null || ngoId.Value == Guid.Empty)
        {
            ThrowError("NGO id is empty");
        }

        var sql =
            """
            SELECT * FROM "GetNgoObserverGuides"(@electionRoundId, @ngoId, @guideId)
            """;

        var queryArgs = new { electionRoundId, ngoId, guideId = guideId };

        ObserverGuideModel? ngoGuide;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            ngoGuide = await dbConnection.QueryFirstOrDefaultAsync<ObserverGuideModel>(sql, queryArgs);
        }

        if (ngoGuide is null)
        {
            return TypedResults.NotFound();
        }

        if (ngoGuide.GuideType == ObserverGuideType.Document)
        {
            var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                ngoGuide.FilePath!,
                ngoGuide.UploadedFileName!);

            ngoGuide = ngoGuide with
            {
                PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
            };
        }

        return TypedResults.Ok(ngoGuide);
    }

    private async Task<Results<Ok<ObserverGuideModel>, NotFound>> ListObserverGuidesAsObserverAsync(
        Guid electionRoundId,
        Guid guideId,
        CancellationToken ct)
    {
        var observerId = currentUserProvider.GetUserId();
        if (observerId == null || observerId.Value == Guid.Empty)
        {
            ThrowError("Observer id is empty");
        }

        var ngo = await context.MonitoringNgos.AsNoTracking()
            .Where(x => x.MonitoringObservers.Any(mo => mo.ObserverId == observerId))
            .Select(x => new { x.NgoId })
            .FirstOrDefaultAsync(ct);

        if (ngo == null)
        {
            ThrowError("Ngo id is empty");
        }

        var sql =
            """
            SELECT * FROM "GetObserverGuideForObserver"(@electionRoundId, @ngoId, @guideId)
            """;

        var queryArgs = new { electionRoundId, ngoId = ngo.NgoId, guideId };

        ObserverGuideModel? ngoGuide;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            ngoGuide = await dbConnection.QueryFirstOrDefaultAsync<ObserverGuideModel>(sql, queryArgs);
        }
        
        if (ngoGuide is null)
        {
            return TypedResults.NotFound();
        }

        if (ngoGuide.GuideType == ObserverGuideType.Document)
        {
            var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                ngoGuide.FilePath!,
                ngoGuide.UploadedFileName!);

            ngoGuide = ngoGuide with
            {
                PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
            };
        }


        return TypedResults.Ok(ngoGuide);
    }
}
