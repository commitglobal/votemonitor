using Vote.Monitor.Domain.Specifications;

namespace Feature.PollingStation.Information.PsiData;

internal static class PsiDataQuery
{
    /// <summary>The polling station information rows matching the filters of <see cref="BuildArgs"/>.</summary>
    public const string Source = """
        "GetPollingStationInformationData"(@electionRoundId, NULL, NULL, NULL, NULL, NULL,
            NULL, @isCompleted, @fromDate, @toDate, @searchText)
        WHERE (CARDINALITY(@level1) = 0 OR "Level1" = ANY(@level1))
          AND (CARDINALITY(@level2) = 0 OR "Level2" = ANY(@level2))
          AND (CARDINALITY(@level3) = 0 OR "Level3" = ANY(@level3))
          AND (CARDINALITY(@level4) = 0 OR "Level4" = ANY(@level4))
          AND (CARDINALITY(@level5) = 0 OR "Level5" = ANY(@level5))
          AND (CARDINALITY(@monitoringNgoIds) = 0 OR "MonitoringNgoId" = ANY(@monitoringNgoIds))
        """;

    public const string OrderBy = """
        ORDER BY
            CASE WHEN @sortExpression = 'LastUpdatedAt ASC' THEN "LastUpdatedAt" END ASC,
            CASE WHEN @sortExpression = 'LastUpdatedAt DESC' THEN "LastUpdatedAt" END DESC,
            CASE WHEN @sortExpression = 'Number ASC' THEN "Number" END ASC,
            CASE WHEN @sortExpression = 'Number DESC' THEN "Number" END DESC,
            CASE WHEN @sortExpression = 'NgoName ASC' THEN "NgoName" END ASC,
            CASE WHEN @sortExpression = 'NgoName DESC' THEN "NgoName" END DESC,
            CASE WHEN @sortExpression = 'Location ASC' THEN "Level1" END ASC,
            CASE WHEN @sortExpression = 'Location DESC' THEN "Level1" END DESC,
            "LastUpdatedAt" DESC,
            "Id"
        """;

    public static object BuildArgs(PsiDataFilters req, bool paged) => new
    {
        electionRoundId = req.ElectionRoundId,
        level1 = NonEmpty(req.Level1Filter),
        level2 = NonEmpty(req.Level2Filter),
        level3 = NonEmpty(req.Level3Filter),
        level4 = NonEmpty(req.Level4Filter),
        level5 = NonEmpty(req.Level5Filter),
        monitoringNgoIds = req.MonitoringNgoId ?? [],
        isCompleted = req.IsCompleted,
        fromDate = req.FromDateFilter?.ToString("O"),
        toDate = req.ToDateFilter?.ToString("O"),
        searchText = string.IsNullOrWhiteSpace(req.SearchText) ? null : $"%{req.SearchText.Trim()}%",
        sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
        offset = paged ? PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber) : 0,
        pageSize = paged ? req.PageSize : int.MaxValue
    };

    /// <summary>The filter's values without the blank ones; empty means no filter.</summary>
    private static string[] NonEmpty(string[]? values) =>
        values?.Where(value => !string.IsNullOrWhiteSpace(value)).ToArray() ?? [];

    private static string GetSortExpression(string? sortColumnName, bool isAscending)
    {
        var order = isAscending ? "ASC" : "DESC";
        var column = sortColumnName?.Trim().ToLowerInvariant() switch
        {
            "number" => "Number",
            "ngoname" => "NgoName",
            "location" or "level1" => "Location",
            "lastupdatedat" => "LastUpdatedAt",
            _ => null
        };

        return column is null ? "LastUpdatedAt DESC" : $"{column} {order}";
    }
}
