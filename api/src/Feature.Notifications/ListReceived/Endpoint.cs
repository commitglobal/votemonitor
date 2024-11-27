using Authorization.Policies;
using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Notifications.ListReceived;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) :
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
        var sql = """
        SELECT
            NGO."Name" AS "NgoName"
        FROM
            "MonitoringObservers" MO
            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
            INNER JOIN "Ngos" NGO ON NGO."Id" = MN."NgoId"
        WHERE
            MO."ObserverId" = @observerId
            AND MO."ElectionRoundId" = @electionRoundId
        FETCH FIRST
            1 ROWS ONLY;

        SELECT
            N."Id",
            N."Title",
            N."Body",
            U."DisplayName" "Sender",
            N."CreatedOn" "SentAt",
            MON."IsRead"
        FROM
            "Observers" O
            INNER JOIN "MonitoringObservers" MO ON MO."ObserverId" = O."Id"
            INNER JOIN "MonitoringObserverNotification" MON ON MON."MonitoringObserverId" = MO."Id"
            INNER JOIN "Notifications" N ON MON."NotificationId" = N."Id"
            INNER JOIN "AspNetUsers" U on U."Id" =  N."SenderId"
        WHERE
            O."Id" = @observerId
            AND N."ElectionRoundId" = @electionRoundId
        ORDER BY
            N."CreatedOn" DESC;
        """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            observerId = req.ObserverId
        };

        string? ngoName;
        List<ReceivedNotificationModel> notifications;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            ngoName = multi.Read<string>().SingleOrDefault();
            notifications = multi.Read<ReceivedNotificationModel>().ToList();
        }
        return TypedResults.Ok(new Response
        {
            NgoName = ngoName,
            Notifications = notifications.ToList()
        });
    }
}
