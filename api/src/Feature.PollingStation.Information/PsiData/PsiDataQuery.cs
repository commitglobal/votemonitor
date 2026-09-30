using Vote.Monitor.Domain.Specifications;

namespace Feature.PollingStation.Information.PsiData;

internal static class PsiDataQuery
{
    public const string FromWhere = """
        FROM "PollingStationInformation" PSI
            INNER JOIN "PollingStations" PS ON PS."Id" = PSI."PollingStationId"
            INNER JOIN "MonitoringObservers" MO ON MO."Id" = PSI."MonitoringObserverId"
            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
            INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
        WHERE PSI."ElectionRoundId" = @electionRoundId
            AND (@level1 IS NULL OR PS."Level1" = @level1)
            AND (@level2 IS NULL OR PS."Level2" = @level2)
            AND (@level3 IS NULL OR PS."Level3" = @level3)
            AND (@level4 IS NULL OR PS."Level4" = @level4)
            AND (@level5 IS NULL OR PS."Level5" = @level5)
            AND (@monitoringNgoId IS NULL OR MN."Id" = @monitoringNgoId)
            AND (@isCompleted IS NULL OR PSI."IsCompleted" = @isCompleted)
            AND (@fromDate IS NULL OR PSI."LastUpdatedAt" >= @fromDate::timestamp)
            AND (@toDate IS NULL OR PSI."LastUpdatedAt" <= @toDate::timestamp)
            AND (@searchText IS NULL OR PS."Number" ILIKE @searchText OR PS."Address" ILIKE @searchText)
        """;

    public const string SelectColumns = """
        SELECT
            PSI."Id",
            PS."Id" AS "PollingStationId",
            PS."Level1",
            PS."Level2",
            PS."Level3",
            PS."Level4",
            PS."Level5",
            PS."Number",
            PS."Address",
            MN."Id" AS "MonitoringNgoId",
            N."Name" AS "NgoName",
            PSI."ArrivalTime",
            PSI."DepartureTime",
            COALESCE("ComputeMinutesMonitoring"(PSI."ArrivalTime", PSI."DepartureTime", PSI."Breaks"), 0) AS "MinutesMonitoring",
            PSI."NumberOfQuestionsAnswered",
            PSI."NumberOfFlaggedAnswers",
            PSI."IsCompleted",
            PSI."LastUpdatedAt"
        """;

    public const string OrderBy = """
        ORDER BY
            CASE WHEN @sortExpression = 'LastUpdatedAt ASC' THEN PSI."LastUpdatedAt" END ASC,
            CASE WHEN @sortExpression = 'LastUpdatedAt DESC' THEN PSI."LastUpdatedAt" END DESC,
            CASE WHEN @sortExpression = 'Number ASC' THEN PS."Number" END ASC,
            CASE WHEN @sortExpression = 'Number DESC' THEN PS."Number" END DESC,
            CASE WHEN @sortExpression = 'NgoName ASC' THEN N."Name" END ASC,
            CASE WHEN @sortExpression = 'NgoName DESC' THEN N."Name" END DESC,
            CASE WHEN @sortExpression = 'Location ASC' THEN PS."Level1" END ASC,
            CASE WHEN @sortExpression = 'Location DESC' THEN PS."Level1" END DESC,
            PSI."LastUpdatedAt" DESC,
            PSI."Id"
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
