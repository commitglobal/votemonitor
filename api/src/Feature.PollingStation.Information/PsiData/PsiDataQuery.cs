using Vote.Monitor.Domain.Specifications;

namespace Feature.PollingStation.Information.PsiData;

internal static class PsiDataQuery
{
    /// <summary>The polling station information rows matching the filters of <see cref="BuildArgs"/>.</summary>
    public const string Source = """
        "GetPollingStationInformationData"(@electionRoundId, @level1, @level2, @level3, @level4, @level5,
            @monitoringNgoId, @isCompleted, @fromDate, @toDate, @searchText)
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
        level1 = NullIfEmpty(req.Level1Filter),
        level2 = NullIfEmpty(req.Level2Filter),
        level3 = NullIfEmpty(req.Level3Filter),
        level4 = NullIfEmpty(req.Level4Filter),
        level5 = NullIfEmpty(req.Level5Filter),
        monitoringNgoId = req.MonitoringNgoId,
        isCompleted = req.IsCompleted,
        fromDate = req.FromDateFilter?.ToString("O"),
        toDate = req.ToDateFilter?.ToString("O"),
        searchText = string.IsNullOrWhiteSpace(req.SearchText) ? null : $"%{req.SearchText.Trim()}%",
        sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
        offset = paged ? PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber) : 0,
        pageSize = paged ? req.PageSize : int.MaxValue
    };

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

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
