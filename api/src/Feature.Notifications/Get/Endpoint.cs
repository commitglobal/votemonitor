using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Notifications.Get;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<NotificationDetailedModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notifications/{id}");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
    }

    public override async Task<Results<Ok<NotificationDetailedModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
        SELECT
            N."Id",
            N."Title",
            N."Body",
            N."CreatedOn" "SentAt",
            U."FirstName" || ' ' || U."LastName" "Sender",
            (
                SELECT
                    JSONB_AGG(
                        JSONB_BUILD_OBJECT(
                            'Id',
                            "MonitoringObserverId",
                            'Name',
                            MOU."FirstName" || ' ' || MOU."LastName",
                            'HasReadNotification',
                            MON."IsRead"
                        )
                    )
                FROM
                    "MonitoringObserverNotification" MON
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = MON."MonitoringObserverId"
                    INNER JOIN "AspNetUsers" MOU ON MOU."Id" = MO."ObserverId"
                WHERE
                    MON."NotificationId" = N."Id"
            ) "Receivers"
        FROM
            "Notifications" N
            INNER JOIN "NgoAdmins" NA ON N."SenderId" = NA."Id"
            INNER JOIN "MonitoringNgos" MN ON NA."NgoId" = MN."NgoId"
            INNER JOIN "AspNetUsers" U ON U."Id" = NA."Id"
        WHERE
            MN."NgoId" = @ngoId
            AND N."ElectionRoundId" = @electionRoundId
            AND N."Id" = @id
        """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            id = req.Id,
        };

        NotificationDetailedModel notification;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            notification = await dbConnection
                .QueryFirstOrDefaultAsync<NotificationDetailedModel>(sql, queryArgs);
        }
        return notification == null ? TypedResults.NotFound() : TypedResults.Ok(notification);
    }
}
