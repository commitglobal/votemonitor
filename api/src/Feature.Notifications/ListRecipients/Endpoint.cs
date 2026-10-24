using Microsoft.AspNetCore.Authorization;
using Authorization.Policies.Requirements;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Notifications.ListRecipients;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory) :
    Endpoint<Request, Results<Ok<PagedResponse<TargetedMonitoringObserverModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notifications:listRecipients");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<PagedResponse<TargetedMonitoringObserverModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql =
            """
            SELECT COUNT(*)
            FROM "GetNotificationRecipientObserverIds"(@electionRoundId, @ngoId, @searchText, @formType, @level1,
                                                       @level2, @level3, @level4, @level5, @pollingStationNumber,
                                                       @hasFlaggedAnswers, @submissionsFollowUpStatus, @tagsFilter,
                                                       @monitoringObserverStatus, @formId, @hasNotes, @hasAttachments,
                                                       @questionsAnswered, @fromDate, @toDate, @hasQuickReports,
                                                       @quickReportFollowUpStatus, @quickReportIncidentCategory, FALSE) R
            WHERE CARDINALITY(@monitoringObserverIds) = 0 OR R."MonitoringObserverId" = ANY(@monitoringObserverIds);

            SELECT "MonitoringObserverId" AS "Id", "ObserverName", "PhoneNumber", "Email", "Tags", "Status"
            FROM "GetNotificationRecipients"(@electionRoundId, @ngoId, @searchText, @formType, @level1, @level2,
                                             @level3, @level4, @level5, @pollingStationNumber, @hasFlaggedAnswers,
                                             @submissionsFollowUpStatus, @tagsFilter, @monitoringObserverStatus,
                                             @formId, @hasNotes, @hasAttachments, @questionsAnswered, @fromDate,
                                             @toDate, @hasQuickReports, @quickReportFollowUpStatus,
                                             @quickReportIncidentCategory)
            WHERE CARDINALITY(@monitoringObserverIds) = 0 OR "MonitoringObserverId" = ANY(@monitoringObserverIds)
            ORDER BY
                CASE WHEN @sortExpression = 'ObserverName ASC' THEN "ObserverName" END ASC,
                CASE WHEN @sortExpression = 'ObserverName DESC' THEN "ObserverName" END DESC,
                CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN "PhoneNumber" END ASC,
                CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN "PhoneNumber" END DESC,
                CASE WHEN @sortExpression = 'Email ASC' THEN "Email" END ASC,
                CASE WHEN @sortExpression = 'Email DESC' THEN "Email" END DESC,
                CASE WHEN @sortExpression = 'Tags ASC' THEN "Tags" END ASC,
                CASE WHEN @sortExpression = 'Tags DESC' THEN "Tags" END DESC,
                CASE WHEN @sortExpression = 'Status ASC' THEN "Status" END ASC,
                CASE WHEN @sortExpression = 'Status DESC' THEN "Status" END DESC
            OFFSET @offset ROWS
            FETCH NEXT @pageSize ROWS ONLY;
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            monitoringObserverIds = req.MonitoringObserverIds ?? [],
            formType = req.FormTypeFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            submissionsFollowUpStatus = req.SubmissionsFollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
            monitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            formId = req.FormId,
            hasNotes = req.HasNotes,
            hasAttachments = req.HasAttachments,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),

            hasQuickReports = req.HasQuickReports,
            quickReportFollowUpStatus = req.QuickReportFollowUpStatus?.ToString(),
            quickReportIncidentCategory = req.QuickReportIncidentCategory?.ToString(),

            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,

            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<TargetedMonitoringObserverModel> entries;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<TargetedMonitoringObserverModel>().ToList();
        }

        return TypedResults.Ok(new PagedResponse<TargetedMonitoringObserverModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(TargetedMonitoringObserverModel.ObserverName)} ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.ObserverName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.Tags),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(TargetedMonitoringObserverModel.Status),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(TargetedMonitoringObserverModel.Status)} {sortOrder}";
        }

        return $"{nameof(TargetedMonitoringObserverModel.ObserverName)} ASC";
    }
}
