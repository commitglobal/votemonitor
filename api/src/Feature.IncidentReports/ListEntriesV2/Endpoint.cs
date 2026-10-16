using Dapper;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Domain.Specifications;

namespace Feature.IncidentReports.ListEntriesV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<IncidentReportEntryModel>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-reports:byEntryV2");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(x => { x.Summary = "Lists incident reports by entry (v2)"; });
    }

    public override async Task<Results<Ok<PagedResponse<IncidentReportEntryModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var compiler = new FilterSqlCompiler(V2ReportFilterFields.IncidentReports);
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
                        FROM "GetIncidentReportEntries"(@electionRoundId, @ngoId, @dataSource) s
                        WHERE {filter.Sql}
                        """;

        var selectSql = $"""
                         SELECT s."Id" AS "IncidentReportId",
                                s."TimeSubmitted",
                                s."FormCode",
                                s."FormStatus",
                                s."FormName",
                                s."FormDefaultLanguage",
                                s."LocationType",
                                s."LocationDescription",
                                s."Level1",
                                s."Level2",
                                s."Level3",
                                s."Level4",
                                s."Level5",
                                s."Number" AS "PollingStationNumber",
                                s."DisplayName" AS "ObserverName",
                                s."Email",
                                s."PhoneNumber",
                                s."Tags",
                                s."NumberOfQuestionsAnswered",
                                s."NumberOfFlaggedAnswers",
                                s."MediaFilesCount",
                                s."NotesCount",
                                s."CommentsCount",
                                s."FollowUpStatus",
                                s."IsCompleted"
                         FROM "GetIncidentReportEntries"(@electionRoundId, @ngoId, @dataSource) s
                         WHERE {filter.Sql}
                         ORDER BY {orderBy}, s."Id"
                         LIMIT @pageSize OFFSET @offset
                         """;

        var sql = $"{countSql};\n{selectSql}";

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);

        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<IncidentReportEntryModel>().ToList();

        return TypedResults.Ok(
            new PagedResponse<IncidentReportEntryModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"s.""TimeSubmitted"" DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.TimeSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""TimeSubmitted"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""FormCode"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.NumberOfQuestionsAnswered),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NumberOfQuestionsAnswered"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NumberOfFlaggedAnswers"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.MediaFilesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""MediaFilesCount"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.NotesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NotesCount"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level1"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level2"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level3"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level4"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level5"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.PollingStationNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Number"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(IncidentReportEntryModel.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""DisplayName"" {sortOrder}";
        }

        return @"s.""TimeSubmitted"" DESC";
    }
}
