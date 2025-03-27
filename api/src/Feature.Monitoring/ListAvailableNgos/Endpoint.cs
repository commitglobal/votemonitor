using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Monitoring.ListAvailableNgos;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<AvailableNgoModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-ngos:available");
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
        Summary(s =>
        {
            s.Summary = "Lists ngos that have status Activated and currently are not monitoring specified election round";
        });
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<AvailableNgoModel>>, NotFound>> ExecuteAsync(Request request,
        CancellationToken ct)
    {
        var sql = """
                  SELECT
                      COUNT(*)
                  FROM
                      "Ngos" N
                  WHERE
                      "Status" = 'Activated'
                    AND "Id" NOT IN (
                      SELECT
                          "NgoId"
                      FROM
                          "MonitoringNgos"
                      WHERE
                          "ElectionRoundId" = @electionRoundId
                  )
                    AND (
                      @searchText IS NULL
                          OR N."Name" ILIKE @searchText
                          OR N."Id"::TEXT ILIKE @searchText
                      );

                  SELECT
                      N."Id",
                      N."Name"
                  FROM
                      "Ngos" N
                  WHERE
                      "Status" = 'Activated'
                    AND "Id" NOT IN (
                      SELECT
                          "NgoId"
                      FROM
                          "MonitoringNgos"
                      WHERE
                          "ElectionRoundId" = @electionRoundId
                  )
                    AND (
                      @searchText IS NULL
                          OR N."Name" ILIKE @searchText
                          OR N."Id"::TEXT ILIKE @searchText
                      )
                  ORDER BY
                      CASE
                          WHEN @sortExpression = 'Name ASC' THEN "Name"
                          END ASC,
                      CASE
                          WHEN @sortExpression = 'Name DESC' THEN "Name"
                          END DESC
                  OFFSET
                      @offset ROWS
                  FETCH NEXT
                      @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            electionRoundId = request.ElectionRoundId,
            searchText = $"%{request.SearchText?.Trim() ?? string.Empty}%",
            offset = PaginationHelper.CalculateSkip(request.PageSize, request.PageNumber),
            pageSize = request.PageSize,
            sortExpression = GetSortExpression(request.SortColumnName, request.IsAscendingSorting)
        };

        int totalRowCount;
        List<AvailableNgoModel> entries;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<AvailableNgoModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<AvailableNgoModel>(entries, totalRowCount, request.PageNumber, request.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return "Name ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(AvailableNgoModel.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(AvailableNgoModel.Name)} {sortOrder}";
        }

        return "Name ASC";
    }
}
