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
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<CitizenNotificationModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminOrStaffRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT * FROM "GetNgoCitizenNotification"(@electionRoundId, @ngoId, @id)
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            id = req.Id
        };

        CitizenNotificationModel? notification;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            notification = await dbConnection
                .QueryFirstOrDefaultAsync<CitizenNotificationModel>(sql, queryArgs);
        }

        return notification == null ? TypedResults.NotFound() : TypedResults.Ok(notification);
    }
}
