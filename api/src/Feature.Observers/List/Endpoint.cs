using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Observers.List;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Ok<PagedResponse<ObserverModel>>>
{
    public override void Configure()
    {
        Get("/api/observers");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Ok<PagedResponse<ObserverModel>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
                  select count(*)
                  from "GetPlatformObserversList"(@searchText, NULL, @isEmailVerified)
                  WHERE CARDINALITY(@statuses) = 0 OR "Status" = ANY(@statuses);

                  select *
                  from "GetPlatformObserversList"(@searchText, NULL, @isEmailVerified)
                  WHERE CARDINALITY(@statuses) = 0 OR "Status" = ANY(@statuses)
                  ORDER BY CASE WHEN @sortExpression = 'Email ASC' THEN "Email" END ASC,
                           CASE WHEN @sortExpression = 'Email DESC' THEN "Email" END DESC,
                           CASE WHEN @sortExpression = 'FirstName ASC' THEN "FirstName" END ASC,
                           CASE WHEN @sortExpression = 'FirstName DESC' THEN "FirstName" END DESC,
                           CASE WHEN @sortExpression = 'LastName ASC' THEN "LastName" END ASC,
                           CASE WHEN @sortExpression = 'LastName DESC' THEN "LastName" END DESC,
                           CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN "PhoneNumber" END ASC,
                           CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN "PhoneNumber" END DESC,
                           CASE WHEN @sortExpression = 'Status ASC' THEN "Status" END ASC,
                           CASE WHEN @sortExpression = 'Status DESC' THEN "Status" END DESC,
                           CASE WHEN @sortExpression = 'IsAccountVerified ASC' THEN "IsAccountVerified" END ASC,
                           CASE WHEN @sortExpression = 'IsAccountVerified DESC' THEN "IsAccountVerified" END DESC,
                           CASE WHEN @sortExpression = 'MonitoredElections ASC' THEN JSONB_ARRAY_LENGTH("MonitoredElections") END ASC,
                           CASE WHEN @sortExpression = 'MonitoredElections DESC' THEN JSONB_ARRAY_LENGTH("MonitoredElections") END DESC
                  OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            statuses = req.Status?.Select(x => x.Value).ToArray() ?? [],
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            isemailverified = req.IsEmailVerified,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<ObserverModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<ObserverModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<ObserverModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(ObserverModel.Email)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(ObserverModel.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverModel.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverModel.FirstName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverModel.FirstName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverModel.LastName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverModel.LastName)} {sortOrder}";
        }
        
        if (string.Equals(sortColumnName, nameof(ObserverModel.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverModel.PhoneNumber)} {sortOrder}";
        }
 
        if (string.Equals(sortColumnName, nameof(ObserverModel.Status),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverModel.Status)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverModel.IsAccountVerified),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverModel.IsAccountVerified)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverModel.MonitoredElections),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverModel.MonitoredElections)} {sortOrder}";
        }

        return $"{nameof(ObserverModel.Email)} DESC";
    }
}
