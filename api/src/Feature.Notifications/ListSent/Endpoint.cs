using Microsoft.AspNetCore.Authorization;
using Authorization.Policies.Requirements;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Notifications.ListSent;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<NotificationModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notifications:listSent");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<PagedResponse<NotificationModel>>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT "CountNgoSentNotifications"(@electionRoundId, @ngoId);

                  SELECT *
                  FROM "GetNgoSentNotifications"(@electionRoundId, @ngoId)
                  ORDER BY "SentAt" DESC
                  OFFSET @offset ROWS
                  FETCH NEXT @pageSize ROWS ONLY;
                  """;
        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize
        };

        int totalRowCount;
        List<NotificationModel> entries = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<NotificationModel>().ToList();
        }

        return TypedResults.Ok(new PagedResponse<NotificationModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }
}
