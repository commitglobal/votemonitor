using Microsoft.AspNetCore.Authorization;
using Authorization.Policies.Requirements;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.MonitoringObservers.List;
public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<PagedResponse<MonitoringObserverModel>>, NotFound>>
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

        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<PagedResponse<MonitoringObserverModel>>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
        SELECT COUNT(*) count
        FROM "GetMonitoringObserversList"(@electionRoundId, @ngoId, @searchText, @tagsFilter, @status);

        SELECT
            "Id",
            "DisplayName",
            "FirstName",
            "LastName",
            "PhoneNumber",
            "Email",
            "Tags",
            "Status",
            "LatestActivityAt",
            "InvitationToken"
        FROM "GetMonitoringObserversList"(@electionRoundId, @ngoId, @searchText, @tagsFilter, @status)

        ORDER BY
            CASE WHEN @sortExpression = 'DisplayName ASC' THEN "DisplayName" END ASC,
            CASE WHEN @sortExpression = 'DisplayName DESC' THEN "DisplayName" END DESC,

            CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN "PhoneNumber" END ASC,
            CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN "PhoneNumber" END DESC,

            CASE WHEN @sortExpression = 'Email ASC' THEN "Email" END ASC,
            CASE WHEN @sortExpression = 'Email DESC' THEN "Email" END DESC,

            CASE WHEN @sortExpression = 'Tags ASC' THEN "Tags" END ASC,
            CASE WHEN @sortExpression = 'Tags DESC' THEN "Tags" END DESC,

            CASE WHEN @sortExpression = 'LatestActivityAt ASC' THEN "LatestActivityAt" END ASC,
            CASE WHEN @sortExpression = 'LatestActivityAt DESC' THEN "LatestActivityAt" END DESC,

            CASE WHEN @sortExpression = 'Status ASC' THEN "Status" END ASC,
            CASE WHEN @sortExpression = 'Status DESC' THEN "Status" END DESC,

            -- a stable order for the rows with the same sort value, so pages do not overlap
            "DisplayName" ASC,
            "Id" ASC

        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;
        """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            tagsFilter = req.Tags ?? [],
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status = req.Status?.ToString(),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<MonitoringObserverModel> entries = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<MonitoringObserverModel>().ToList();
        }
        return TypedResults.Ok(new PagedResponse<MonitoringObserverModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return "DisplayName ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        // the web app's observer name column sorts by display name
        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.DisplayName), StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(sortColumnName, "ObserverName", StringComparison.InvariantCultureIgnoreCase))
        {
            return $"DisplayName {sortOrder}";
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

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.LatestActivityAt), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.LatestActivityAt)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(MonitoringObserverModel.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(MonitoringObserverModel.Status)} {sortOrder}";
        }

        return "DisplayName ASC";
    }
}
