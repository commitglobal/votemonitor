using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.ElectionRounds.List;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<Response>>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<Response>>, ProblemDetails>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = """
                  SELECT COUNT(*)
                  FROM "GetElectionRoundOverviews"(@searchText, NULL, NULL)
                  WHERE (CARDINALITY(@electionRoundStatuses) = 0 OR "Status" = ANY(@electionRoundStatuses))
                    AND (CARDINALITY(@countryIds) = 0 OR "CountryId" = ANY(@countryIds));

                  SELECT *
                  FROM "GetElectionRoundOverviews"(@searchText, NULL, NULL)
                  WHERE (CARDINALITY(@electionRoundStatuses) = 0 OR "Status" = ANY(@electionRoundStatuses))
                    AND (CARDINALITY(@countryIds) = 0 OR "CountryId" = ANY(@countryIds))
                  ORDER BY CASE WHEN @sortExpression = 'Title ASC' THEN "Title" END ASC,
                           CASE WHEN @sortExpression = 'Title DESC' THEN "Title" END DESC,
                           CASE WHEN @sortExpression = 'EnglishTitle ASC' THEN "EnglishTitle" END ASC,
                           CASE WHEN @sortExpression = 'EnglishTitle DESC' THEN "EnglishTitle" END DESC,
                           CASE WHEN @sortExpression = 'StartDate ASC' THEN "StartDate" END ASC,
                           CASE WHEN @sortExpression = 'StartDate DESC' THEN "StartDate" END DESC,
                           CASE WHEN @sortExpression = 'Status ASC' THEN "Status" END ASC,
                           CASE WHEN @sortExpression = 'Status DESC' THEN "Status" END DESC,
                           CASE WHEN @sortExpression = 'CountryName ASC' THEN "CountryName" END ASC,
                           CASE WHEN @sortExpression = 'CountryName DESC' THEN "CountryName" END DESC,
                           CASE WHEN @sortExpression = 'MonitoringNgos ASC' THEN JSONB_ARRAY_LENGTH("MonitoringNgos") END ASC,
                           CASE WHEN @sortExpression = 'MonitoringNgos DESC' THEN JSONB_ARRAY_LENGTH("MonitoringNgos") END DESC
                  OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            electionRoundStatuses = req.ElectionRoundStatus?.Select(x => x.Value).ToArray() ?? [],
            countryIds = req.CountryId ?? [],
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount = 0;
        List<Response> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<Response>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<Response>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(List.Response.StartDate)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";


        if (string.Equals(sortColumnName, nameof(List.Response.Title),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(List.Response.Title)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(List.Response.EnglishTitle),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(List.Response.EnglishTitle)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(List.Response.StartDate),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(List.Response.StartDate)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(List.Response.CountryName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(List.Response.CountryName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(List.Response.MonitoringNgos),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(List.Response.MonitoringNgos)} {sortOrder}";
        }

        return $"{nameof(List.Response.StartDate)} DESC";
    }
}
