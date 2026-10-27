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

    private static readonly string RecordsWithObserverEmailAndSubmissionNumber =
        CorrectableRecordsSql.WithObserverEmailAndSubmissionNumber("""
            "GetCorrectableRecords"(@electionRoundId, NULL, @recordId, @monitoringNgoId, @searchText,
                @pollingStationNumber, @correctedOnly, NULL, NULL, NULL, NULL, NULL)
            """);

    public override async Task<PagedResponse<CorrectableRecordModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = $$"""
                  SELECT COUNT(*)
                  FROM "GetCorrectableRecords"(@electionRoundId, NULL, @recordId, @monitoringNgoId, @searchText,
                      @pollingStationNumber, @correctedOnly, NULL, NULL, NULL, NULL, NULL)
                  WHERE (CARDINALITY(@recordTypes) = 0 OR "RecordType" = ANY(@recordTypes))
                    AND (CARDINALITY(@level1) = 0 OR "Level1" = ANY(@level1))
                    AND (CARDINALITY(@level2) = 0 OR "Level2" = ANY(@level2))
                    AND (CARDINALITY(@level3) = 0 OR "Level3" = ANY(@level3))
                    AND (CARDINALITY(@level4) = 0 OR "Level4" = ANY(@level4))
                    AND (CARDINALITY(@level5) = 0 OR "Level5" = ANY(@level5));

                  SELECT *
                  FROM {{RecordsWithObserverEmailAndSubmissionNumber}}
                  WHERE (CARDINALITY(@recordTypes) = 0 OR "RecordType" = ANY(@recordTypes))
                    AND (CARDINALITY(@level1) = 0 OR "Level1" = ANY(@level1))
                    AND (CARDINALITY(@level2) = 0 OR "Level2" = ANY(@level2))
                    AND (CARDINALITY(@level3) = 0 OR "Level3" = ANY(@level3))
                    AND (CARDINALITY(@level4) = 0 OR "Level4" = ANY(@level4))
                    AND (CARDINALITY(@level5) = 0 OR "Level5" = ANY(@level5))
                  ORDER BY
                      CASE WHEN @sortExpression = 'SubmittedAt ASC' THEN "SubmittedAt" END ASC,
                      CASE WHEN @sortExpression = 'SubmittedAt DESC' THEN "SubmittedAt" END DESC,
                      CASE WHEN @sortExpression = 'ObserverName ASC' THEN "ObserverName" END ASC,
                      CASE WHEN @sortExpression = 'ObserverName DESC' THEN "ObserverName" END DESC,
                      CASE WHEN @sortExpression = 'PollingStationNumber ASC' THEN "Number" END ASC,
                      CASE WHEN @sortExpression = 'PollingStationNumber DESC' THEN "Number" END DESC,
                      "Id"
                  OFFSET @offset ROWS
                  FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var args = new
        {
            electionRoundId = req.ElectionRoundId,
            recordTypes = req.RecordType?.Select(x => x.Value).Distinct().ToArray() ?? [],
            recordId = (Guid?)null,
            monitoringNgoId = req.MonitoringNgoId,
            searchText = ToContainsPattern(req.SearchText),
            pollingStationNumber = ToContainsPattern(req.PollingStationNumberFilter),
            correctedOnly = req.CorrectedOnly,
            level1 = NonEmpty(req.Level1Filter),
            level2 = NonEmpty(req.Level2Filter),
            level3 = NonEmpty(req.Level3Filter),
            level4 = NonEmpty(req.Level4Filter),
            level5 = NonEmpty(req.Level5Filter),
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

    /// <summary>The filter's values without the blank ones; empty means no filter.</summary>
    private static string[] NonEmpty(string[]? values) =>
        values?.Where(value => !string.IsNullOrWhiteSpace(value)).ToArray() ?? [];

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
