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
        SELECT * FROM "GetObserverNotificationsNgoName"(@electionRoundId, @observerId);

        SELECT *
        FROM "GetObserverReceivedNotifications"(@electionRoundId, @observerId)
        ORDER BY "SentAt" DESC;
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
