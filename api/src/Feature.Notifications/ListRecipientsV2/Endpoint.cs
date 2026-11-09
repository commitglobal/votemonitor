using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Notifications.ListRecipients;
using Feature.Notifications.RecipientsV2;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Notifications.ListRecipientsV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory) :
    Endpoint<Request, Results<Ok<PagedResponse<TargetedMonitoringObserverModel>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notifications:listRecipientsV2");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(x =>
        {
            x.Summary = "Lists notification recipients matching a v2 filter";
            x.Description =
                "The NGO's monitoring observers matching a v2 filter over the observer and its form submissions " +
                "(fs:), quick reports (qr:) and incident reports (ir:).";
        });
    }

    public override async Task<Results<Ok<PagedResponse<TargetedMonitoringObserverModel>>, NotFound>> ExecuteAsync(
        Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var parameters = new DynamicParameters();
        parameters.Add("offset", PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber));
        parameters.Add("pageSize", req.PageSize);
        var recipients = RecipientsV2Query.With(req, parameters);

        var sql = $"""
                   {recipients}
                   SELECT COUNT(*) FROM "Recipients";

                   {recipients}
                   SELECT O."Id", O."DisplayName" AS "ObserverName", O."PhoneNumber", O."Email"::TEXT, O."Tags", O."Status"
                   FROM "Recipients" R
                       INNER JOIN "Observers" O ON O."Id" = R."MonitoringObserverId"
                   ORDER BY {GetOrderByClause(req.SortColumnName, req.IsAscendingSorting)}, O."Id"
                   LIMIT @pageSize OFFSET @offset;
                   """;

        int totalRowCount;
        List<TargetedMonitoringObserverModel> entries;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<TargetedMonitoringObserverModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<TargetedMonitoringObserverModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetOrderByClause(string? sortColumnName, bool isAscendingSorting)
    {
        var sortOrder = isAscendingSorting ? "ASC" : "DESC";
        var column = sortColumnName?.ToLowerInvariant() switch
        {
            "email" => "O.\"Email\"",
            "phonenumber" => "O.\"PhoneNumber\"",
            "tags" => "O.\"Tags\"",
            "status" => "O.\"Status\"",
            _ => "O.\"DisplayName\""
        };

        return $"{column} {sortOrder}";
    }
}
