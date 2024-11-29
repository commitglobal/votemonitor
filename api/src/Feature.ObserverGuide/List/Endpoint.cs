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

        var isNgoAdmin = currentUserRoleProvider.IsNgoAdmin();
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
            SELECT
            	G."Id",
            	G."Title",
            	G."FileName",
            	G."MimeType",
            	G."GuideType",
            	G."Text",
            	G."WebsiteUrl",
            	G."LastModifiedOn",
            	G."LastModifiedBy",
            	G."IsGuideOwner",
            	G."FilePath",
            	G."UploadedFileName",
            	CASE
            		WHEN G."IsGuideOwner" THEN COALESCE(
            			(
            				SELECT
            					JSONB_AGG(
            						JSONB_BUILD_OBJECT('NgoId', N."Id", 'Name', N."Name")
            					)
            				FROM
            					"CoalitionGuideAccess" CFA
            					INNER JOIN "Coalitions" C ON C."Id" = CFA."CoalitionId"
            					INNER JOIN "MonitoringNgos" MN ON CFA."MonitoringNgoId" = MN."Id"
            					INNER JOIN "Ngos" N ON MN."NgoId" = N."Id"
            				WHERE
            					C."ElectionRoundId" = @electionRoundId
            					AND CFA."GuideId" = G."Id"
            			),
            			'[]'::JSONB
            		)
            		ELSE '[]'::JSONB
            	END AS "GuideAccess"
            FROM
            	(
            		SELECT
            			G."Id",
            			G."Title",
            			G."FileName",
            			G."MimeType",
            			G."GuideType",
            			G."Text",
            			G."WebsiteUrl",
            			COALESCE(G."LastModifiedOn", G."CreatedOn") AS "LastModifiedOn",
            			COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
            			EXISTS (
            				SELECT
            					1
            				FROM
            					"GetMonitoringNgoDetails" (@electionRoundId, @ngoId)
            				WHERE
            					"MonitoringNgoId" = G."MonitoringNgoId"
            			) AS "IsGuideOwner",
            		    G."FilePath",
                        G."UploadedFileName"
            		FROM
            			"CoalitionGuideAccess" CGA
            			INNER JOIN "Coalitions" C ON CGA."CoalitionId" = C."Id"
            			INNER JOIN "ObserversGuides" G ON CGA."GuideId" = G."Id"
            			INNER JOIN "AspNetUsers" CREATOR ON G."CreatedBy" = CREATOR."Id"
            			LEFT JOIN "AspNetUsers" UPDATER ON G."LastModifiedBy" = UPDATER."Id"
            		WHERE
            			CGA."MonitoringNgoId" = (
            				SELECT
            					"MonitoringNgoId"
            				FROM
            					"GetMonitoringNgoDetails" (@electionRoundId, @ngoId)
            			)
            			AND C."ElectionRoundId" = @electionRoundId
            			AND (
            				(
            					SELECT
            						"CoalitionId" IS NOT NULL
            					FROM
            						"GetMonitoringNgoDetails" (@electionRoundId, @ngoId)
            				)
            				OR (
            					SELECT
            						"IsCoalitionLeader"
            					FROM
            						"GetMonitoringNgoDetails" (@electionRoundId, @ngoId)
            				)
            			)
            		UNION
            		SELECT
            			G."Id",
            			G."Title",
            			G."FileName",
            			G."MimeType",
            			G."GuideType",
            			G."Text",
            			G."WebsiteUrl",
            			COALESCE(G."LastModifiedOn", G."CreatedOn") AS "LastModifiedOn",
            			COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
            			TRUE AS "IsGuideOwner",
            			G."FilePath",
                        G."UploadedFileName"
            		FROM
            			"ObserversGuides" G
            			INNER JOIN "MonitoringNgos" MN ON G."MonitoringNgoId" = MN."Id"
            			INNER JOIN "AspNetUsers" CREATOR ON G."CreatedBy" = CREATOR."Id"
            			LEFT JOIN "AspNetUsers" UPDATER ON G."LastModifiedBy" = UPDATER."Id"
            		WHERE
            			MN."ElectionRoundId" = @electionRoundId
            			AND G."MonitoringNgoId" = (
            				SELECT
            					"MonitoringNgoId"
            				FROM
            					"GetMonitoringNgoDetails" (@electionRoundId, @ngoId)
            			)
            	) G
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

        var ngo = await context.MonitoringNgos
            .Where(x => x.MonitoringObservers.Any(mo => mo.ObserverId == observerId))
            .Select(x => new { x.NgoId })
            .FirstOrDefaultAsync(ct);

        if (ngo == null)
        {
            ThrowError("Ngo id is empty");
        }
        
        var sql =
            """
            SELECT
            	G."Id",
            	G."Title",
            	G."FileName",
            	G."MimeType",
            	G."GuideType",
            	G."Text",
            	G."WebsiteUrl",
            	G."FilePath",
            	G."UploadedFileName",
            	COALESCE(G."LastModifiedOn", G."CreatedOn") AS "LastModifiedOn",
            	n."Name" AS "LastModifiedBy",
            	EXISTS (
            		SELECT
            			1
            		FROM
            			"GetMonitoringNgoDetails" (@electionRoundId, @ngoId)
            		WHERE
            			"MonitoringNgoId" = G."MonitoringNgoId"
            	) AS "IsGuideOwner"
            FROM
            	"CoalitionGuideAccess" CGA
            	INNER JOIN "Coalitions" C ON CGA."CoalitionId" = C."Id"
            	INNER JOIN "ObserversGuides" G ON CGA."GuideId" = G."Id"
                INNER JOIN "MonitoringNgos" mn on g."MonitoringNgoId" = mn."Id"
                INNER JOIN "Ngos" n on n."Id" = mn."NgoId"
            WHERE
            	CGA."MonitoringNgoId" = (
            		SELECT
            			"MonitoringNgoId"
            		FROM
            			"GetMonitoringNgoDetails" (@electionRoundId, @ngoId)
            	)
            	AND C."ElectionRoundId" = @electionRoundId
            	AND (
            		(
            			SELECT
            				"CoalitionId" IS NOT NULL
            			FROM
            				"GetMonitoringNgoDetails" (@electionRoundId, @ngoId)
            		)
            		OR (
            			SELECT
            				"IsCoalitionLeader"
            			FROM
            				"GetMonitoringNgoDetails" (@electionRoundId, @ngoId)
            		)
            	)
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
