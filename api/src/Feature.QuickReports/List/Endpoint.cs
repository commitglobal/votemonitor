using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Microsoft.AspNetCore.Authorization;
using Authorization.Policies.Requirements;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.QuickReports.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<QuickReportOverviewModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/quick-reports");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Summary(s =>
        {
            s.Summary = "Gets all quick-reports submitted by observers for a monitoring ngo";
        });
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<PagedResponse<QuickReportOverviewModel>>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
        SELECT
            COUNT(*) AS "TotalNumberOfRows"
        FROM
            "GetQuickReportOverviews"(
                @electionRoundId, @ngoId, @dataSource, @coalitionMemberId, @monitoringObserverId, @tagsFilter,
                @followUpStatus, @stageId, @reviewStatus, @reviewerId, @hasReviewer, @severities, @quickReportLocationType,
                @incidentCategory, @searchText, @level1, @level2, @level3, @level4, @level5, @pollingStationId, @fromDate, @toDate,
                @hasAttachments, @hasComments);

        SELECT
            QR.*
        FROM
            "GetQuickReportOverviews"(
                @electionRoundId, @ngoId, @dataSource, @coalitionMemberId, @monitoringObserverId, @tagsFilter,
                @followUpStatus, @stageId, @reviewStatus, @reviewerId, @hasReviewer, @severities, @quickReportLocationType,
                @incidentCategory, @searchText, @level1, @level2, @level3, @level4, @level5, @pollingStationId, @fromDate, @toDate,
                @hasAttachments, @hasComments) QR
        ORDER BY
            CASE WHEN @sortExpression = 'Severity ASC' THEN "SeverityRank"(QR."Severity") END ASC,
            CASE WHEN @sortExpression = 'Severity DESC' THEN "SeverityRank"(QR."Severity") END DESC,
            CASE WHEN @sortExpression LIKE 'Severity %' THEN QR."Timestamp" END DESC,
            CASE WHEN @sortExpression = 'Timestamp ASC' THEN QR."Timestamp" END ASC,
            CASE WHEN @sortExpression = 'Timestamp DESC' THEN QR."Timestamp" END DESC,
            CASE WHEN @sortExpression = 'NumberOfAttachments ASC' THEN QR."NumberOfAttachments" END ASC,
            CASE WHEN @sortExpression = 'NumberOfAttachments DESC' THEN QR."NumberOfAttachments" END DESC
        OFFSET
            @offset ROWS
        FETCH NEXT
            @pageSize ROWS ONLY;
        """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            ngoId = req.NgoId,
            coalitionMemberId = req.CoalitionMemberId,
            dataSource = req.DataSource.ToString(),
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            stageId = req.StageId,
            reviewStatus = req.ReviewStatus?.Value,
            reviewerId = req.ReviewerId,
            hasReviewer = req.HasReviewer,
            severities = req.Severity?.Select(x => x.Value).ToArray() ?? [],
            followUpStatus = req.QuickReportFollowUpStatus?.ToString(),
            quickReportLocationType = req.QuickReportLocationType?.ToString(),
            incidentCategory = req.IncidentCategory?.ToString(),
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),
            hasAttachments = req.HasAttachments,
            hasComments = req.HasComments,
            monitoringObserverId = req.MonitoringObserverId,
            pollingStationId = req.PollingStationId,
            tagsFilter = req.TagsFilter ?? [],
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<QuickReportOverviewModel> entries = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<QuickReportOverviewModel>().ToList();
        }

        return TypedResults.Ok(new PagedResponse<QuickReportOverviewModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (SeveritySql.IsSortColumn(sortColumnName))
        {
            // By level (Urgent highest), ties in the list's default order.
            return $"Severity {(isAscendingSorting ? "ASC" : "DESC")}";
        }

        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(QuickReportOverviewModel.Timestamp)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(QuickReportOverviewModel.Timestamp), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(QuickReportOverviewModel.Timestamp)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(QuickReportOverviewModel.NumberOfAttachments),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(QuickReportOverviewModel.NumberOfAttachments)} {sortOrder}";
        }

        return $"{nameof(QuickReportOverviewModel.Timestamp)} ASC";
    }
}
