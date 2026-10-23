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
        var orderBy = GetOrderByClause(req.SortColumnName, req.IsAscendingSorting);

        var sql = $"""
                   SELECT COUNT(*) AS COUNT
                   FROM "GetNgoOverviews"(@searchText, @status);

                   SELECT *
                   FROM "GetNgoOverviews"(@searchText, @status)
                   ORDER BY {orderBy}
                   OFFSET @offset ROWS
                   FETCH NEXT @pageSize ROWS ONLY;
                   """;

        var queryArgs = new
        {
            searchText = string.IsNullOrWhiteSpace(req.SearchText) ? null : $"%{req.SearchText.Trim()}%",
            status = req.Status?.ToString(),
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize
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
