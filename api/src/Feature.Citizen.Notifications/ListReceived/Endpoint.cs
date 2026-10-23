using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Citizen.Notifications.ListReceived;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) :
    Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-notifications:listReceived");
        DontAutoTag();
        Options(x => x.WithTags("citizen-notifications", "mobile"));
        AllowAnonymous();
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
                  SELECT * FROM "GetCitizenNotificationsNgoName"(@electionRoundId);

                  SELECT *
                  FROM "GetElectionRoundCitizenNotifications"(@electionRoundId)
                  ORDER BY "SentAt" DESC;
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId
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