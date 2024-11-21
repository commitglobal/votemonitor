using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Citizen.Notifications.ListSent;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, IAuthorizationService authorizationService)
    : Endpoint<Request, Results<Ok<PagedResponse<CitizenNotificationModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-notifications:listSent");
        DontAutoTag();
        Options(x => x.WithTags("citizen-notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<CitizenNotificationModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT
                  	count(CN."Id")
                  FROM
                    "CitizenNotifications" CN
                  	INNER JOIN "ElectionRounds" ER ON CN."ElectionRoundId" = ER."Id"
                  	INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                  WHERE
                      MN."NgoId" = @ngoId
                      AND CN."ElectionRoundId" = @electionRoundId;

                  SELECT
                      CN."Id",
                      CN."Title",
                      CN."Body",
                      CN."CreatedOn" AS "SentAt",
                      U."DisplayName" AS "Sender"
                  FROM
                      "CitizenNotifications" CN
                      INNER JOIN "ElectionRounds" ER ON CN."ElectionRoundId" = ER."Id"
                      INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                      INNER JOIN "AspNetUsers" U ON U."Id" = CN."SenderId"
                  WHERE
                      MN."NgoId" = @ngoId
                      AND CN."ElectionRoundId" = @electionRoundId
                      
                  ORDER BY CN."CreatedOn" DESC
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
        List<CitizenNotificationModel> entries;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<CitizenNotificationModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<CitizenNotificationModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }
}
