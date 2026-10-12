using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListAll;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<CitizenReportAdminRowModel>>
{
    private const string FromWhere = """
        FROM "CitizenReports" CR
            INNER JOIN "Forms" F ON F."Id" = CR."FormId"
            INNER JOIN "Locations" L ON L."Id" = CR."LocationId"
            INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
            INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
        WHERE CR."ElectionRoundId" = @electionRoundId
            AND (@level1 IS NULL OR L."Level1" = @level1)
            AND (@level2 IS NULL OR L."Level2" = @level2)
            AND (@level3 IS NULL OR L."Level3" = @level3)
            AND (@level4 IS NULL OR L."Level4" = @level4)
            AND (@level5 IS NULL OR L."Level5" = @level5)
            AND (@monitoringNgoId IS NULL OR MN."Id" = @monitoringNgoId)
            AND (@formId IS NULL OR CR."FormId" = @formId)
            AND (@fromDate IS NULL OR COALESCE(CR."LastModifiedOn", CR."CreatedOn") >= @fromDate::timestamp)
            AND (@toDate IS NULL OR COALESCE(CR."LastModifiedOn", CR."CreatedOn") <= @toDate::timestamp)
            AND (@searchText IS NULL OR F."Code" ILIKE @searchText OR L."Level1" ILIKE @searchText
                 OR L."Level2" ILIKE @searchText OR L."Level3" ILIKE @searchText)
        """;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports:all");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s => { s.Summary = "Lists all citizen reports in an election round (platform admins)"; });
    }

    public override async Task<PagedResponse<CitizenReportAdminRowModel>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = $"""
                   SELECT COUNT(*)
                   {FromWhere};

                   SELECT
                       CR."Id" AS "CitizenReportId",
                       COALESCE(CR."LastModifiedOn", CR."CreatedOn") AS "TimeSubmitted",
                       F."Id" AS "FormId",
                       F."Code" AS "FormCode",
                       F."Name" AS "FormName",
                       F."DefaultLanguage" AS "FormDefaultLanguage",
                       L."Id" AS "LocationId",
                       L."Level1",
                       L."Level2",
                       L."Level3",
                       L."Level4",
                       L."Level5",
                       MN."Id" AS "MonitoringNgoId",
                       N."Name" AS "NgoName",
                       CR."NumberOfQuestionsAnswered",
                       CR."NumberOfFlaggedAnswers",
                       CR."FollowUpStatus"
                   {FromWhere}
                   ORDER BY
                       CASE WHEN @sortExpression = 'TimeSubmitted ASC' THEN COALESCE(CR."LastModifiedOn", CR."CreatedOn") END ASC,
                       CASE WHEN @sortExpression = 'TimeSubmitted DESC' THEN COALESCE(CR."LastModifiedOn", CR."CreatedOn") END DESC,
                       CASE WHEN @sortExpression = 'FormCode ASC' THEN F."Code" END ASC,
                       CASE WHEN @sortExpression = 'FormCode DESC' THEN F."Code" END DESC,
                       CASE WHEN @sortExpression = 'NgoName ASC' THEN N."Name" END ASC,
                       CASE WHEN @sortExpression = 'NgoName DESC' THEN N."Name" END DESC,
                       CASE WHEN @sortExpression = 'Location ASC' THEN L."Level1" END ASC,
                       CASE WHEN @sortExpression = 'Location DESC' THEN L."Level1" END DESC,
                       COALESCE(CR."LastModifiedOn", CR."CreatedOn") DESC,
                       CR."Id"
                   OFFSET @offset ROWS
                   FETCH NEXT @pageSize ROWS ONLY;
                   """;

        var args = new
        {
            electionRoundId = req.ElectionRoundId,
            level1 = NullIfEmpty(req.Level1Filter),
            level2 = NullIfEmpty(req.Level2Filter),
            level3 = NullIfEmpty(req.Level3Filter),
            level4 = NullIfEmpty(req.Level4Filter),
            level5 = NullIfEmpty(req.Level5Filter),
            monitoringNgoId = req.MonitoringNgoId,
            formId = req.FormId,
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),
            searchText = string.IsNullOrWhiteSpace(req.SearchText) ? null : $"%{req.SearchText.Trim()}%",
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, args);

        var totalCount = multi.Read<int>().Single();
        var items = multi.Read<CitizenReportAdminRowModel>().ToList();

        return new PagedResponse<CitizenReportAdminRowModel>(items, totalCount, req.PageNumber, req.PageSize);
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    private static string GetSortExpression(string? sortColumnName, bool isAscending)
    {
        var order = isAscending ? "ASC" : "DESC";
        var column = sortColumnName?.Trim().ToLowerInvariant() switch
        {
            "timesubmitted" => "TimeSubmitted",
            "formcode" => "FormCode",
            "ngoname" => "NgoName",
            "location" or "level1" => "Location",
            _ => null
        };

        return column is null ? "TimeSubmitted DESC" : $"{column} {order}";
    }
}
