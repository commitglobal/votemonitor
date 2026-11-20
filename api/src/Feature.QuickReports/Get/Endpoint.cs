using Vote.Monitor.Domain.Entities.RecordLabelAggregate;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.QuickReports.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    ICurrentUserRoleProvider userRoleProvider,
    ICurrentUserProvider userProvider,
    VoteMonitorContext context)
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
        if (userRoleProvider.IsPlatformAdmin())
        {
            return await GetQuickReportAsPlatformAdminAsync(req.ElectionRoundId, req.Id, ct);
        }

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

        var followUp = await context.GetFollowUpStatusAsync(electionRoundId, ngoId, ReportRecordType.QuickReport, quickReportId, ct);
        var reviewTarget = await context.GetReviewTargetAsync(electionRoundId, ngoId, ReportRecordType.QuickReport, quickReportId, ct);
        var reviewWorkflow = reviewTarget is null ? null : await context.GetReviewWorkflowAsync(reviewTarget, ct);
        var recordSeverity = reviewTarget is null ? null : await context.GetRecordSeverityAsync(reviewTarget, ct);
        var recordLabels = reviewTarget is null ? null : await context.GetRecordLabelsAsync(reviewTarget, ct);
        return TypedResults.Ok(submission with
        {
            FollowUpStatusId = followUp?.Id,
            FollowUpStatusName = followUp?.Name,
            FollowUpStatusColor = followUp?.Color,
            FollowUpStatusIcon = followUp?.Icon,
            ReviewStage = reviewWorkflow?.Stage,
            StageChangedAt = reviewWorkflow?.StageChangedAt,
            ReviewedAt = reviewWorkflow?.ReviewedAt,
            Reviewers = reviewWorkflow?.Reviewers,
            Severity = recordSeverity?.Severity,
            SeverityChangedAt = recordSeverity?.ChangedAt,
            Labels = recordLabels
        });
    }

    /// <summary>
    /// Read as the NGO whose observer sent it, then left without what platform admins mustn't see (ForPlatformAdmin).
    /// </summary>
    private async Task<Results<Ok<QuickReportDetailedModel>, NotFound>> GetQuickReportAsPlatformAdminAsync(
        Guid electionRoundId, Guid quickReportId, CancellationToken ct)
    {
        var owner = await context.GetRecordOwnerAsync(electionRoundId, ReportRecordType.QuickReport, quickReportId, ct);
        if (owner is null)
        {
            return TypedResults.NotFound();
        }

        const string sql = """
                           SELECT *
                           FROM "GetNgoQuickReportDetails"(@electionRoundId, @ngoId, @quickReportId)
                           """;

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        var report = await dbConnection.QueryFirstOrDefaultAsync<QuickReportDetailedModel>(sql,
            new { electionRoundId, ngoId = owner.NgoId, quickReportId });

        return report is null ? TypedResults.NotFound() : TypedResults.Ok(report.ForPlatformAdmin(owner.NgoName));
    }

    private async Task SetPresignUrls(QuickReportDetailedModel submission)
    {
        foreach (var attachment in submission.Attachments ?? [])
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
