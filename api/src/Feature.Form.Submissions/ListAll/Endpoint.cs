using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListAll;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<FormSubmissionAdminRowModel>>
{
    private const string Source = """
        "GetElectionRoundFormSubmissionRows"(@electionRoundId, NULL, NULL, NULL, NULL, NULL,
            @monitoringNgoId, @formId, @isCompleted, @fromDate, @toDate, @searchText)
        WHERE (CARDINALITY(@level1) = 0 OR "Level1" = ANY(@level1))
          AND (CARDINALITY(@level2) = 0 OR "Level2" = ANY(@level2))
          AND (CARDINALITY(@level3) = 0 OR "Level3" = ANY(@level3))
          AND (CARDINALITY(@level4) = 0 OR "Level4" = ANY(@level4))
          AND (CARDINALITY(@level5) = 0 OR "Level5" = ANY(@level5))
        """;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:all");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Lists all form submissions in an election round with anonymized observer data (platform admins)";
        });
    }

    public override async Task<PagedResponse<FormSubmissionAdminRowModel>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = $"""
                   SELECT COUNT(*)
                   FROM {Source};

                   SELECT *
                   FROM {Source}
                   ORDER BY
                       CASE WHEN @sortExpression = 'TimeSubmitted ASC' THEN "TimeSubmitted" END ASC,
                       CASE WHEN @sortExpression = 'TimeSubmitted DESC' THEN "TimeSubmitted" END DESC,
                       CASE WHEN @sortExpression = 'FormCode ASC' THEN "FormCode" END ASC,
                       CASE WHEN @sortExpression = 'FormCode DESC' THEN "FormCode" END DESC,
                       CASE WHEN @sortExpression = 'Number ASC' THEN "Number" END ASC,
                       CASE WHEN @sortExpression = 'Number DESC' THEN "Number" END DESC,
                       CASE WHEN @sortExpression = 'NgoName ASC' THEN "NgoName" END ASC,
                       CASE WHEN @sortExpression = 'NgoName DESC' THEN "NgoName" END DESC,
                       CASE WHEN @sortExpression = 'Location ASC' THEN "Level1" END ASC,
                       CASE WHEN @sortExpression = 'Location DESC' THEN "Level1" END DESC,
                       "TimeSubmitted" DESC,
                       "SubmissionId"
                   OFFSET @offset ROWS
                   FETCH NEXT @pageSize ROWS ONLY;
                   """;

        var args = new
        {
            electionRoundId = req.ElectionRoundId,
            level1 = NonEmpty(req.Level1Filter),
            level2 = NonEmpty(req.Level2Filter),
            level3 = NonEmpty(req.Level3Filter),
            level4 = NonEmpty(req.Level4Filter),
            level5 = NonEmpty(req.Level5Filter),
            monitoringNgoId = req.MonitoringNgoId,
            formId = req.FormId,
            isCompleted = req.IsCompleted,
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
        var items = multi.Read<FormSubmissionAdminRowModel>().Select(x => x.WithMaskedObserverData()).ToList();

        return new PagedResponse<FormSubmissionAdminRowModel>(items, totalCount, req.PageNumber, req.PageSize);
    }

    /// <summary>The filter's values without the blank ones; empty means no filter.</summary>
    private static string[] NonEmpty(string[]? values) =>
        values?.Where(value => !string.IsNullOrWhiteSpace(value)).ToArray() ?? [];

    private static string GetSortExpression(string? sortColumnName, bool isAscending)
    {
        var order = isAscending ? "ASC" : "DESC";
        var column = sortColumnName?.Trim().ToLowerInvariant() switch
        {
            "timesubmitted" => "TimeSubmitted",
            "formcode" => "FormCode",
            "number" => "Number",
            "ngoname" => "NgoName",
            "location" or "level1" => "Location",
            _ => null
        };

        return column is null ? "TimeSubmitted DESC" : $"{column} {order}";
    }
}
