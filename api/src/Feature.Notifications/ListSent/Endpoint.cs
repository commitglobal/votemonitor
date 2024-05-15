using System.Data;
using Dapper;

namespace Feature.Notifications.ListSent;

public class Endpoint(IDbConnection dbConnection) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notifications:listSent");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        SELECT
            N.""Id"",
            N.""Title"",
            N.""Body"",
            N.""CreatedOn"" ""SentAt"",
            U.""FirstName"" || ' ' || U.""LastName"" ""Sender"",
            (
                SELECT
                    JSONB_AGG(
                        JSONB_BUILD_OBJECT(
                            'Id',
                            ""TargetedObserversId"",
                            'Name',
                            MOU.""FirstName"" || ' ' || MOU.""LastName""
                        )
                    )
                FROM
                    ""MonitoringObserverNotification"" MON
                    INNER JOIN ""MonitoringObservers"" MO ON MO.""Id"" = MON.""TargetedObserversId""
                    INNER JOIN ""AspNetUsers"" MOU ON MOU.""Id"" = MO.""ObserverId""
                WHERE
                    MON.""NotificationId"" = N.""Id""
            ) ""Receivers""
        FROM
            ""Notifications"" N
            INNER JOIN ""NgoAdmins"" NA ON N.""SenderId"" = NA.""Id""
            INNER JOIN ""MonitoringNgos"" MN ON NA.""NgoId"" = MN.""NgoId""
            INNER JOIN ""AspNetUsers"" U ON U.""Id"" = NA.""Id""
        WHERE
            MN.""NgoId"" = @ngoId
            AND N.""ElectionRoundId"" = @electionRoundId
        ORDER BY N.""CreatedOn"" DESC";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
        };

        var notifications = await dbConnection.QueryAsync<NotificationModel>(sql, queryArgs);

        return TypedResults.Ok(new Response
        {
            Notifications = notifications.ToList()
        });
    }
}
