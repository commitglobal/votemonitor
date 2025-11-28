using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.QuickReports.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    ICurrentUserRoleProvider userRoleProvider,
    ICurrentUserProvider userProvider)
    : Endpoint<Request, Results<Ok<QuickReportDetailedModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/quick-reports/{id}");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets details regarding a quick report";
            s.Description = "Gets a quick report with it's attachments and refreshed presigned urls";
        });
    }

    public override async Task<Results<Ok<QuickReportDetailedModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new MonitoringNgoAdminOrObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        if (userRoleProvider.IsObserver())
        {
            return await GetQuickReportAsObserverAsync(req.ElectionRoundId, userProvider.GetUserId()!.Value, req.Id,
                ct);
        }

        if (userRoleProvider.IsNgoAdmin())
        {
            return await GetQuickReportAsNgoAdminAsync(req.ElectionRoundId, userProvider.GetNgoId()!.Value, req.Id, ct);
        }

        return TypedResults.NotFound();
    }

    private async Task<Results<Ok<QuickReportDetailedModel>, NotFound>> GetQuickReportAsNgoAdminAsync(
        Guid electionRoundId, Guid ngoId, Guid quickReportId, CancellationToken ct)
    {
        var sql = """
                  SELECT qr."Id",
                         qr."ElectionRoundId",
                         qr."QuickReportLocationType",
                         QR."LastUpdatedAt" as "Timestamp",
                         qr."Title",
                         qr."Description",
                         qr."MonitoringObserverId",
                         AMO."DisplayName" as "ObserverName",
                         AMO."PhoneNumber",
                         AMO."Email",
                         AMO."Tags",
                         AMO."NgoName",
                         AMO."IsOwnObserver",
                         qr."PollingStationId",
                         ps."Level1",
                         ps."Level2",
                         ps."Level3",
                         ps."Level4",
                         ps."Level5",
                         ps."Number",
                         ps."Address",
                         qr."PollingStationDetails",
                         qr."IncidentCategory",
                         qr."FollowUpStatus",
                         COALESCE((select jsonb_agg(jsonb_build_object('QuickReportId', qr."Id", 'FileName', qra."FileName", 'MimeType', qra."MimeType", 'FilePath', qra."FilePath", 'UploadedFileName', qra."UploadedFileName", 'TimeSubmitted', qra."LastUpdatedAt"))
                                   FROM "QuickReportAttachments" qra
                                   WHERE
                                       qra."ElectionRoundId" = @electionRoundId
                                     AND qra."MonitoringObserverId" = qr."MonitoringObserverId"
                                     and qra."QuickReportId" = @quickReportId
                                     AND qra."IsDeleted" = false AND qra."IsCompleted" = true),'[]'::JSONB) AS "Attachments"
                         
                  FROM "QuickReports" QR
                           INNER JOIN "MonitoringObservers" mo on mo."Id" = qr."MonitoringObserverId"
                           INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, 'Coalition') AMO on AMO."MonitoringObserverId" = qr."MonitoringObserverId"
                           LEFT JOIN "PollingStations" ps on ps."Id" = qr."PollingStationId"
                  WHERE qr."ElectionRoundId" = @electionRoundId
                    and qr."Id" = @quickReportId
                  """;

        var queryArgs = new { electionRoundId, quickReportId, ngoId };

        QuickReportDetailedModel submission = null;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            submission = await dbConnection.QueryFirstOrDefaultAsync<QuickReportDetailedModel>(sql, queryArgs);
        }

        if (submission is null)
        {
            return TypedResults.NotFound();
        }

        await SetPresignUrls(submission);

        return TypedResults.Ok(submission);
    }

    private async Task SetPresignUrls(QuickReportDetailedModel submission)
    {
        foreach (var attachment in submission.Attachments)
        {
            var result =
                await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
            if (result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds))
            {
                attachment.PresignedUrl = url;
                attachment.UrlValidityInSeconds = urlValidityInSeconds;
            }
        }
    }

    private async Task<Results<Ok<QuickReportDetailedModel>, NotFound>> GetQuickReportAsObserverAsync(
        Guid electionRoundId, Guid observerId, Guid quickReportId, CancellationToken ct)
    {
        var sql = """
                  SELECT qr."Id",
                         qr."ElectionRoundId",
                         qr."QuickReportLocationType",
                         qr."LastUpdatedAt" as "Timestamp",
                         qr."Title",
                         qr."Description",
                         qr."PollingStationId",
                         ps."Level1",
                         ps."Level2",
                         ps."Level3",
                         ps."Level4",
                         ps."Level5",
                         ps."Number",
                         ps."Address",
                         qr."PollingStationDetails",
                         qr."IncidentCategory",
                         qr."FollowUpStatus",
                         COALESCE((select jsonb_agg(jsonb_build_object('QuickReportId', qr."Id", 'FileName', qra."FileName", 'MimeType', qra."MimeType", 'FilePath', qra."FilePath", 'UploadedFileName', qra."UploadedFileName", 'TimeSubmitted', qra."LastUpdatedAt"))
                                   FROM "QuickReportAttachments" qra
                                   WHERE
                                       qra."ElectionRoundId" = @electionRoundId
                                     AND qra."MonitoringObserverId" = qr."MonitoringObserverId"
                                     AND qra."QuickReportId"  = @quickReportId
                                     AND qra."IsDeleted" = false AND qra."IsCompleted" = true),'[]'::JSONB) AS "Attachments"
                         
                  FROM "QuickReports" QR
                           INNER JOIN "MonitoringObservers" mo on mo."Id" = qr."MonitoringObserverId"
                           INNER JOIN "AspNetUsers" u on u."Id" = mo."ObserverId"
                           LEFT JOIN "PollingStations" ps on ps."Id" = qr."PollingStationId"
                  WHERE qr."ElectionRoundId" = @electionRoundId
                    and qr."Id" = @quickReportId
                    and mo."ObserverId" = @observerId;
                  """;

        var queryArgs = new { electionRoundId, quickReportId, observerId };

        QuickReportDetailedModel? submission = null!;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            submission = await dbConnection.QueryFirstOrDefaultAsync<QuickReportDetailedModel>(sql, queryArgs);
        }

        if (submission is null)
        {
            return TypedResults.NotFound();
        }

        await SetPresignUrls(submission);
        
        return TypedResults.Ok(submission);
    }
}
