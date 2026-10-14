using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Ngos.List;

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
        var builder = new SqlBuilder();
        builder.AddParameters(new
        {
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize
        });

        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            builder.Where(@"(N.""Name"" ILIKE @searchText OR N.""Id""::TEXT ILIKE @searchText)",
                new { searchText = $"%{req.SearchText.Trim()}%" });
        }

        if (req.Status is not null)
        {
            builder.Where(@"N.""Status"" = @status", new { status = req.Status.ToString() });
        }

        builder.OrderBy(GetOrderByClause(req.SortColumnName, req.IsAscendingSorting));

        var countTemplate = builder.AddTemplate(
            """
            SELECT COUNT(*) AS COUNT
            FROM "Ngos" N
            /**where**/
            """);

        var selectTemplate = builder.AddTemplate(
            """
            WITH CTE_Ngos AS (
                SELECT
                    N."Id",
                    N."Name",
                    N."Status",
                    (SELECT COUNT(1)
                     FROM "NgoAdmins" NA
                     WHERE NA."NgoId" = N."Id") AS "NumberOfNgoAdmins",
                    (SELECT COUNT(1)
                     FROM "NgoStaff" NS
                     WHERE NS."NgoId" = N."Id") AS "NumberOfStaff",
                    (SELECT COUNT(1)
                     FROM "MonitoringNgos" MN
                     WHERE MN."NgoId" = N."Id") AS "NumberOfMonitoredElections",
                    (
                      SELECT MAX(ER."StartDate")
                      FROM "MonitoringNgos" MN
                      INNER JOIN "ElectionRounds" ER ON ER."Id" = MN."ElectionRoundId"
                      WHERE MN."NgoId" = N."Id"
                    ) AS "DateOfLastElection"
                FROM "Ngos" N
                /**where**/
            )
            SELECT * FROM CTE_Ngos
            /**orderby**/
            OFFSET @offset ROWS
            FETCH NEXT @pageSize ROWS ONLY
            """);

        var sql = $"{countTemplate.RawSql};\n{selectTemplate.RawSql}";

        int totalRowCount;
        List<NgoModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, countTemplate.Parameters);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<NgoModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<NgoModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return @"""Name"" ASC";
        }

        if (string.Equals(sortColumnName, nameof(NgoModel.Name),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""Name"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(NgoModel.Status),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""Status"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(NgoModel.NumberOfNgoAdmins),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfNgoAdmins"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(NgoModel.NumberOfStaff),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfStaff"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(NgoModel.NumberOfMonitoredElections),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""NumberOfMonitoredElections"" {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(NgoModel.DateOfLastElection),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $@"""DateOfLastElection"" {sortOrder}";
        }

        return @"""Name"" ASC";
    }
}
