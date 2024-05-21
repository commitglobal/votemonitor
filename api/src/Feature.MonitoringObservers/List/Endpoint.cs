using System.Data;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.MonitoringObservers.List;
public class Endpoint(IDbConnection dbConnection) : Endpoint<Request, PagedResponse<MonitoringObserverModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-observers");
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Lists monitoring observers";
        });

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
            AND (@searchText IS NULL OR @searchText = '' OR (U.""FirstName"" || ' ' || U.""LastName"") ILIKE @searchText OR U.""Email"" ILIKE @searchText OR U.""PhoneNumber"" ILIKE @searchText)
            AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo.""Tags"" @> @tagsFilter)
            AND (@status IS NULL OR  mo.""Status"" = @status);

        SELECT
            ""Id"",
            ""FirstName"",
            ""LastName"",
            ""PhoneNumber"",
            ""Email"",
            ""Tags"",
            ""Status""
        FROM (
            SELECT
                MO.""Id"",
                U.""FirstName"",
                U.""LastName"",
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
                AND (@searchText IS NULL OR @searchText = '' OR (U.""FirstName"" || ' ' || U.""LastName"") ILIKE @searchText OR U.""Email"" ILIKE @searchText OR u.""PhoneNumber"" ILIKE @searchText)
                AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo.""Tags"" @> @tagsFilter)
                AND (@status IS NULL OR  mo.""Status"" = @status)
            ) T

        ORDER BY
            CASE WHEN @sortExpression = 'ObserverName ASC' THEN ""FirstName"" || ' ' || ""LastName"" END ASC,
            CASE WHEN @sortExpression = 'ObserverName DESC' THEN ""FirstName"" || ' ' || ""LastName"" END DESC,

            CASE WHEN @sortExpression = 'FirstName ASC' THEN ""FirstName"" END ASC,
            CASE WHEN @sortExpression = 'FirstName DESC' THEN ""FirstName"" END DESC,

            CASE WHEN @sortExpression = 'LastName ASC' THEN ""LastName"" END ASC,
            CASE WHEN @sortExpression = 'LastName DESC' THEN ""LastName"" END DESC,

            CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN ""PhoneNumber"" END ASC,
            CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN ""PhoneNumber"" END DESC,

            CASE WHEN @sortExpression = 'Email ASC' THEN ""Email"" END ASC,
            CASE WHEN @sortExpression = 'Email DESC' THEN ""Email"" END DESC,

            CASE WHEN @sortExpression = 'Tags ASC' THEN ""Tags"" END ASC,
            CASE WHEN @sortExpression = 'Tags DESC' THEN ""Tags"" END DESC
         
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;"
        ;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            tagsFilter = req.Tags ?? [],
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status = req.Status?.ToString(),
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
            return "ObserverName ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.FirstName), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.FirstName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.LastName), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.LastName)} {sortOrder}";
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

        return "ObserverName ASC";
    }
}
