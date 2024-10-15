using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Citizen.Notifications.Get;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, IAuthorizationService authorizationService)
    : Endpoint<Request, Results<Ok<CitizenNotificationModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-notifications/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CitizenNotificationModel>, NotFound>> ExecuteAsync(Request req,
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
                    CN."Id",
                    CN."Title",
                    CN."Body",
                    CN."CreatedOn" "SentAt",
                    U."FirstName" || ' ' || U."LastName" "Sender"
                  FROM
                    "CitizenNotifications" CN
                    INNER JOIN "NgoAdmins" NA ON CN."SenderId" = NA."Id"
                    INNER JOIN "MonitoringNgos" MN ON NA."NgoId" = MN."NgoId"
                    INNER JOIN "AspNetUsers" U ON U."Id" = NA."Id"
                  WHERE
                    MN."NgoId" = @ngoId
                    AND CN."ElectionRoundId" = @electionRoundId
                    AND CN."Id" = @id
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            id = req.Id,
        };

        CitizenNotificationModel notification;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            notification = await dbConnection
                .QueryFirstOrDefaultAsync<CitizenNotificationModel>(sql, queryArgs);
        }

        return notification == null ? TypedResults.NotFound() : TypedResults.Ok(notification);
    }
}