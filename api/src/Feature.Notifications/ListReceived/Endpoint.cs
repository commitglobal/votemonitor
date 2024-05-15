using System.Data;
using Authorization.Policies;
using Dapper;

namespace Feature.Notifications.ListReceived;

public class Endpoint(IDbConnection dbConnection) :
        Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notifications:listReceived");
        DontAutoTag();
        Options(x => x.WithTags("notifications", "mobile"));
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        SELECT
            N.""Title"",
            N.""Body"",
            U.""FirstName"" || ' ' || U.""LastName"" ""Sender"",
            N.""CreatedOn"" ""SentAt""
        FROM
            ""MonitoringObserverNotification"" MON
            INNER JOIN ""Notifications"" N ON MON.""NotificationId"" = N.""Id""
            INNER JOIN ""NgoAdmins"" NA ON N.""SenderId"" = NA.""Id""
            INNER JOIN ""MonitoringNgos"" MN ON NA.""NgoId"" = MN.""NgoId""
            INNER JOIN ""AspNetUsers"" U ON U.""Id"" = NA.""Id""
            INNER JOIN ""MonitoringObservers"" MO ON MO.""Id"" = MON.""TargetedObserversId""
        WHERE
            MO.""ObserverId"" = @observerId
            AND N.""ElectionRoundId"" = @electionRoundId
        ORDER BY
            N.""CreatedOn"" DESC";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            observerId = req.ObserverId,
        };

        var notifications = await dbConnection.QueryAsync<ReceivedNotificationModel>(sql, queryArgs);

        return TypedResults.Ok(new Response
        {
            Notifications = notifications.ToList()
        });
    }
}
