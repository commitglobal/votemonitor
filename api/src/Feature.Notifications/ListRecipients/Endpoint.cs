using System.Data;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Notifications.ListRecipients;

public class Endpoint(IDbConnection dbConnection) :
        Endpoint<Request, PagedResponse<MonitoringObserverModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notifications:listRecipients");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<PagedResponse<MonitoringObserverModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        SELECT COUNT(*) count
        FROM
            ""MonitoringObservers"" MO
            INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
            INNER JOIN ""Observers"" O ON O.""Id"" = MO.""ObserverId""
            INNER JOIN ""AspNetUsers"" U ON U.""Id"" = O.""ApplicationUserId""
        WHERE
            MN.""ElectionRoundId"" = @electionRoundId
            AND MN.""NgoId"" = @ngoId
            AND (@searchText IS NULL OR @searchText = '' OR u.""FirstName"" ILIKE @searchText OR u.""LastName"" ILIKE @searchText OR u.""Email"" ILIKE @searchText OR u.""PhoneNumber"" ILIKE @searchText)
            AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo.""Tags"" @> @tagsFilter);

        SELECT 
            ""MonitoringObserverId"",
            ""ObserverName"",
            ""PhoneNumber"",
            ""Email"",
            ""Tags"",
            ""Status""
            
        FROM (
            SELECT
                MO.""Id"" ""MonitoringObserverId"",
                U.""FirstName"" || ' ' || U.""LastName"" ""ObserverName"",
                U.""PhoneNumber"",
                U.""Email"",
                MO.""Tags"",
                MO.""Status""
            FROM
                ""MonitoringObservers"" MO
                INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
                INNER JOIN ""Observers"" O ON O.""Id"" = MO.""ObserverId""
                INNER JOIN ""AspNetUsers"" U ON U.""Id"" = O.""ApplicationUserId""
            WHERE
                MN.""ElectionRoundId"" = @electionRoundId
                AND MN.""NgoId"" = @ngoId
                AND (@searchText IS NULL OR @searchText = '' OR u.""FirstName"" ILIKE @searchText OR    u.""LastName"" ILIKE @searchText OR u.""Email"" ILIKE @searchText OR u.""PhoneNumber"" ILIKE   @searchText)
                AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo.""Tags"" @> @tagsFilter)
            ) T

        ORDER BY
            CASE WHEN @sortExpression = 'ObserverName ASC' THEN ""ObserverName"" END ASC,
            CASE WHEN @sortExpression = 'ObserverName DESC' THEN ""ObserverName"" END DESC,

            CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN ""PhoneNumber"" END ASC,
            CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN ""PhoneNumber"" END DESC,

            CASE WHEN @sortExpression = 'Email ASC' THEN ""Email"" END ASC,
            CASE WHEN @sortExpression = 'Email DESC' THEN ""Email"" END DESC,

            CASE WHEN @sortExpression = 'Tags ASC' THEN ""Tags"" END ASC,
            CASE WHEN @sortExpression = 'Tags DESC' THEN ""Tags"" END DESC
         
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            tagsFilter = req.TagsFilter ?? [],
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
        };

        var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<MonitoringObserverModel>().ToList();

        return new PagedResponse<MonitoringObserverModel>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(MonitoringObserverModel.ObserverName)} ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.ObserverName), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.ObserverName)} {sortOrder}";
        }
        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.Email), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.PhoneNumber), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.Tags), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.Tags)} {sortOrder}";
        }

        return $"{nameof(MonitoringObserverModel.ObserverName)} ASC";
    }
}
