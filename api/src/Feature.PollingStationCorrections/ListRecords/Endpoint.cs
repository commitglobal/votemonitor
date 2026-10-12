using Feature.PollingStationCorrections.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.PollingStationCorrections.ListRecords;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<CorrectableRecordModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-station-corrections:records");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-corrections"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Lists the records platform admins can move to another polling station, with their correction history";
        });
    }

    public override async Task<PagedResponse<CorrectableRecordModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = $"""
                   SELECT COUNT(*)
                   {CorrectableRecordsSql.FromWhere};

                   SELECT
                   {CorrectableRecordsSql.Columns}
                   {CorrectableRecordsSql.FromWhere}
                   ORDER BY
                       CASE WHEN @sortExpression = 'SubmittedAt ASC' THEN R."SubmittedAt" END ASC,
                       CASE WHEN @sortExpression = 'SubmittedAt DESC' THEN R."SubmittedAt" END DESC,
                       CASE WHEN @sortExpression = 'ObserverName ASC' THEN U."DisplayName" END ASC,
                       CASE WHEN @sortExpression = 'ObserverName DESC' THEN U."DisplayName" END DESC,
                       CASE WHEN @sortExpression = 'PollingStationNumber ASC' THEN PS."Number" END ASC,
                       CASE WHEN @sortExpression = 'PollingStationNumber DESC' THEN PS."Number" END DESC,
                       R."Id"
                   OFFSET @offset ROWS
                   FETCH NEXT @pageSize ROWS ONLY;
                   """;

        var args = new
        {
            electionRoundId = req.ElectionRoundId,
            recordType = req.RecordType?.Value,
            recordId = (Guid?)null,
            monitoringNgoId = req.MonitoringNgoId,
            searchText = ToContainsPattern(req.SearchText),
            pollingStationNumber = ToContainsPattern(req.PollingStationNumberFilter),
            correctedOnly = req.CorrectedOnly,
            level1 = NullIfEmpty(req.Level1Filter),
            level2 = NullIfEmpty(req.Level2Filter),
            level3 = NullIfEmpty(req.Level3Filter),
            level4 = NullIfEmpty(req.Level4Filter),
            level5 = NullIfEmpty(req.Level5Filter),
            sortExpression = GetSortExpression(req.SortColumnName, req.SortOrder),
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, args);

        var totalCount = multi.Read<int>().Single();
        var items = multi.Read<CorrectableRecordRow>().Select(x => x.ToModel()).ToList();

        return new PagedResponse<CorrectableRecordModel>(items, totalCount, req.PageNumber, req.PageSize);
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    private static string? ToContainsPattern(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : $"%{value.Trim()}%";

    private static string GetSortExpression(string? sortColumnName, SortOrder? sortOrder)
    {
        var column = sortColumnName?.Trim().ToLowerInvariant() switch
        {
            "observername" => "ObserverName",
            "pollingstationnumber" => "PollingStationNumber",
            _ => "SubmittedAt"
        };

        // Newest first unless asked otherwise; names and numbers A to Z.
        var isAscending = sortOrder is null ? column != "SubmittedAt" : sortOrder == SortOrder.Asc;

        return $"{column} {(isAscending ? "ASC" : "DESC")}";
    }
}
