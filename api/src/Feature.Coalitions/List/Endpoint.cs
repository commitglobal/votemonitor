using Dapper;
using Feature.NgoCoalitions.Models;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.NgoCoalitions.List;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<CoalitionModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/coalitions");
        DontAutoTag();
        Options(x => x.WithTags("coalitions"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<CoalitionModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = """
                  SELECT COUNT(*) COUNT
                  FROM "GetElectionRoundCoalitions"(@electionRoundId, @searchText);

                  SELECT "Id", "Name", "Members", "LeaderId", "LeaderName"
                  FROM "GetElectionRoundCoalitions"(@electionRoundId, @searchText)
                  ORDER BY
                      CASE WHEN @sortExpression = 'Name ASC' THEN "Name" END ASC,
                      CASE WHEN @sortExpression = 'Name DESC' THEN "Name" END DESC,
                      CASE WHEN @sortExpression = 'NumberOfMembers ASC' THEN JSONB_ARRAY_LENGTH("Members") END ASC,
                      CASE WHEN @sortExpression = 'NumberOfMembers DESC' THEN JSONB_ARRAY_LENGTH("Members") END DESC
                  OFFSET @offset ROWS
                  FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount = 0;
        List<CoalitionModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<CoalitionModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<CoalitionModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return "Name ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(CoalitionModel.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CoalitionModel.Name)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(CoalitionModel.NumberOfMembers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(CoalitionModel.NumberOfMembers)} {sortOrder}";
        }

        return "Name ASC";
    }
}
