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

        if (userRoleProvider.IsNgoAdmin() || userRoleProvider.IsNgoStaff())
        {
            return await GetQuickReportAsNgoAdminAsync(req.ElectionRoundId, userProvider.GetNgoId()!.Value, req.Id, ct);
        }

        return TypedResults.NotFound();
    }

    private async Task<Results<Ok<QuickReportDetailedModel>, NotFound>> GetQuickReportAsNgoAdminAsync(
        Guid electionRoundId, Guid ngoId, Guid quickReportId, CancellationToken ct)
    {
        var sql = """
                  SELECT *
                  FROM "GetNgoQuickReportDetails"(@electionRoundId, @ngoId, @quickReportId)
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
                  SELECT *
                  FROM "GetObserverQuickReport"(@electionRoundId, @observerId, @quickReportId);
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
