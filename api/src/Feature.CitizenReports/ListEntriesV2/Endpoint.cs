using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Dapper;
using Feature.CitizenReports.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListEntriesV2;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, IAuthorizationService authorizationService)
    : Endpoint<Request, Results<Ok<PagedResponse<CitizenReportEntryModel>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports:byEntryV2");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(x => { x.Summary = "Lists citizen report submissions by entry (v2)"; });
    }

    public override async Task<Results<Ok<PagedResponse<CitizenReportEntryModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var compiler = new FilterSqlCompiler(V2ReportFilterFields.CitizenReports);
        var filter = compiler.Build(req.Filter);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", req.ElectionRoundId);
        parameters.Add("ngoId", req.NgoId);
        parameters.Add("offset", PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber));
        parameters.Add("pageSize", req.PageSize);

        filter.AddTo(parameters);

        var orderBy = GetOrderByClause(req.SortColumnName, req.IsAscendingSorting);

        var countSql = $"""
                        SELECT COUNT(1)
                        FROM "GetCitizenReportEntries"(@electionRoundId, @ngoId) s
                        WHERE {filter.Sql}
                        """;

        var selectSql = $"""
                         SELECT s."Id" AS "CitizenReportId",
                                s."TimeSubmitted",
                                s."FormCode",
                                s."FormStatus",
                                s."FormName",
                                s."FormDefaultLanguage",
                                s."NumberOfQuestionsAnswered",
                                s."NumberOfFlaggedAnswers",
                                s."NotesCount",
                                s."MediaFilesCount",
                                s."CommentsCount",
                                s."FollowUpStatus",
                                s."StageId",
                                s."StageName",
                                s."ReviewStatus",
                                s."StageChangedAt",
                                s."ReviewedAt",
                                s."ReviewerIds",
                                s."Severity",
                                s."Level1",
                                s."Level2",
                                s."Level3",
                                s."Level4",
                                s."Level5"
                         FROM "GetCitizenReportEntries"(@electionRoundId, @ngoId) s
                         WHERE {filter.Sql}
                         ORDER BY {orderBy}, s."Id"
                         LIMIT @pageSize OFFSET @offset
                         """;

        var sql = $"{countSql};\n{selectSql}";

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);

        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<CitizenReportEntryModel>().ToList();

        return TypedResults.Ok(
            new PagedResponse<CitizenReportEntryModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        if (SeveritySql.IsSortColumn(sortColumnName))
        {
            // By level (Urgent highest), ties in the list's default order.
            return $@"""SeverityRank""(s.""Severity"") {(isAscendingSorting ? "ASC" : "DESC")}, s.""TimeSubmitted"" DESC";
        }

        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"s.""TimeSubmitted"" DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""FormCode"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.NumberOfQuestionsAnswered),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NumberOfQuestionsAnswered"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NumberOfFlaggedAnswers"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.MediaFilesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""MediaFilesCount"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.NotesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""NotesCount"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.TimeSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""TimeSubmitted"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level1"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level2"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level3"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level4"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CitizenReportEntryModel.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"s.""Level5"" {sortOrder}";
        }

        return @"s.""TimeSubmitted"" DESC";
    }
}
