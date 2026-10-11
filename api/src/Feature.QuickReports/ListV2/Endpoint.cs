using Microsoft.AspNetCore.Authorization;
using Authorization.Policies.Requirements;
using Authorization.Policies;
using Dapper;
using Feature.QuickReports.List;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Domain.Specifications;

namespace Feature.QuickReports.ListV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<QuickReportOverviewModel>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/quick-reports:listV2");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Summary(s =>
        {
            s.Summary = "Gets all quick-reports submitted by observers for a monitoring ngo (v2)";
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

        var compiler = new FilterSqlCompiler(V2ReportFilterFields.QuickReports);
        var filter = compiler.Build(req.Filter);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", req.ElectionRoundId);
        parameters.Add("ngoId", req.NgoId);
        parameters.Add("dataSource", req.DataSource.ToString());
        parameters.Add("offset", PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber));
        parameters.Add("pageSize", req.PageSize);

        filter.AddTo(parameters);

        var orderBy = GetOrderByClause(req.SortColumnName, req.IsAscendingSorting);

        var countSql = $"""
                        SELECT COUNT(1)
                        FROM "GetQuickReportEntries"(@electionRoundId, @ngoId, @dataSource) s
                        WHERE {filter.Sql}
                        """;

        var selectSql = $"""
                         SELECT s."Id",
                                s."LocationType" AS "QuickReportLocationType",
                                s."TimeSubmitted" AS "Timestamp",
                                s."Title",
                                s."Description",
                                s."IncidentCategory",
                                s."FollowUpStatus",
                                s."NumberOfAttachments",
                                s."CommentsCount",
                                s."MonitoringObserverId",
                                s."DisplayName" AS "ObserverName",
                                s."PhoneNumber",
                                s."Email",
                                s."Tags",
                                s."NgoName",
                                s."PollingStationDetails",
                                s."PollingStationId",
                                s."Level1",
                                s."Level2",
                                s."Level3",
                                s."Level4",
                                s."Level5",
                                s."Number",
                                s."Address"
                         FROM "GetQuickReportEntries"(@electionRoundId, @ngoId, @dataSource) s
                         WHERE {filter.Sql}
                         ORDER BY {orderBy}, s."Id"
                         LIMIT @pageSize OFFSET @offset
                         """;

        var sql = $"{countSql};\n{selectSql}";

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);

        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<QuickReportOverviewModel>().ToList();

        return TypedResults.Ok(new PagedResponse<QuickReportOverviewModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"s.""TimeSubmitted"" DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(QuickReportOverviewModel.Timestamp),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""TimeSubmitted"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(QuickReportOverviewModel.NumberOfAttachments),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NumberOfAttachments"" {sortOrder}";
        }

        return @"s.""TimeSubmitted"" DESC";
    }
}
