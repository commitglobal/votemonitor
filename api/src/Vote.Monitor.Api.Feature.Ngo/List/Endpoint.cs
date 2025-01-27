using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.Ngo.List;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<NgoModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/ngos");
        DontAutoTag();
        Options(x => x.WithTags("ngos"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<NgoModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = """
                  SELECT
                    COUNT(*) AS COUNT
                  FROM
                    "Ngos" N
                  WHERE
                    (
                        @searchText IS NULL
                        OR N."Name" ILIKE @searchText
                        OR N."Id"::TEXT ILIKE @searchText
                    )
                    AND (@status IS NULL OR N."Status" = @status);

                  WITH CTE_Ngos AS (
                      SELECT
                          N."Id",
                          N."Name",
                          N."Status",
                          (SELECT COUNT(1) 
                           FROM "NgoAdmins" NA 
                           WHERE NA."NgoId" = N."Id") AS "NumberOfNgoAdmins",
                          (SELECT COUNT(1) 
                           FROM "MonitoringNgos" MN 
                           WHERE MN."NgoId" = N."Id") AS "NumberOfElectionsMonitoring",
                          (
                            SELECT MAX(ER."StartDate") 
                            FROM "MonitoringNgos" MN
                            INNER JOIN "ElectionRounds" ER ON ER."Id" = MN."ElectionRoundId"
                            WHERE MN."NgoId" = N."Id"
                          ) AS "DateOfLastElection"
                      FROM
                          "Ngos" N
                      WHERE
                          (
                              @searchText IS NULL
                              OR N."Name" ILIKE @searchText
                              OR N."Id"::TEXT ILIKE @searchText
                          )
                          AND (@status IS NULL OR N."Status" = @status)
                  )
                  SELECT * FROM CTE_Ngos
                  ORDER BY
                      CASE
                          WHEN @sortExpression = 'Name ASC' THEN "Name"
                          END ASC,
                      CASE
                          WHEN @sortExpression = 'Name DESC' THEN "Name"
                          END DESC,
                      CASE
                          WHEN @sortExpression = 'Status ASC' THEN "Status"
                          END ASC,
                      CASE
                          WHEN @sortExpression = 'Status DESC' THEN "Status"
                          END DESC,
                      CASE
                          WHEN @sortExpression = 'NumberOfNgoAdmins ASC' THEN "NumberOfNgoAdmins"
                          END ASC,
                      CASE
                          WHEN @sortExpression = 'NumberOfNgoAdmins DESC' THEN "NumberOfNgoAdmins"
                          END DESC,  
                      CASE
                          WHEN @sortExpression = 'NumberOfElectionsMonitoring ASC' THEN "NumberOfElectionsMonitoring"
                          END ASC,
                      CASE
                          WHEN @sortExpression = 'NumberOfElectionsMonitoring DESC' THEN "NumberOfElectionsMonitoring"
                          END DESC,
                      CASE
                          WHEN @sortExpression = 'DateOfLastElection ASC' THEN "DateOfLastElection"
                          END ASC,
                      CASE
                          WHEN @sortExpression = 'DateOfLastElection DESC' THEN "DateOfLastElection"
                          END DESC
                  OFFSET @offset ROWS
                  FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status = req.Status?.ToString(),
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<NgoModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<NgoModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<NgoModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(NgoModel.Name)} ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(NgoModel.Status),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(NgoModel.Status)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(NgoModel.NumberOfNgoAdmins),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(NgoModel.NumberOfNgoAdmins)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(NgoModel.NumberOfElectionsMonitoring),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(NgoModel.NumberOfElectionsMonitoring)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(NgoModel.DateOfLastElection),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(NgoModel.DateOfLastElection)} {sortOrder}";
        }

        return $"{nameof(NgoModel.Name)} DESC";
    }
}
