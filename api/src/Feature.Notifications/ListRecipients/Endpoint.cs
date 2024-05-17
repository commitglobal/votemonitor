using System.Data;
using Authorization.Policies;
using Dapper;

namespace Feature.Notifications.ListRecipients;

public class Endpoint(IDbConnection dbConnection) :
        Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notifications:listRecipients");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        SELECT
            NGO.""Name"" AS ""NgoName""
        FROM
            ""MonitoringObservers"" MO
            INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
            INNER JOIN ""Ngos"" NGO ON NGO.""Id"" = MN.""NgoId""
        WHERE
            MO.""ObserverId"" = @observerId
            AND MO.""ElectionRoundId"" = @electionRoundId
        FETCH FIRST
            1 ROWS ONLY;

        SELECT
            N.""Id"",
            N.""Title"",
            N.""Body"",
            U.""FirstName"" || ' ' || U.""LastName"" ""Sender"",
            N.""CreatedOn"" ""SentAt""
        FROM
            ""Observers"" O
            INNER JOIN ""MonitoringObservers"" MO ON MO.""ObserverId"" = O.""Id""
            INNER JOIN ""MonitoringObserverNotification"" MON ON MON.""TargetedObserversId"" = MO.""Id""
            INNER JOIN ""Notifications"" N ON MON.""NotificationId"" = N.""Id""
            INNER JOIN ""AspNetUsers"" U on U.""Id"" =  N.""SenderId""
        WHERE
            O.""Id"" = @observerId
            AND N.""ElectionRoundId"" = @electionRoundId
        ORDER BY
            N.""CreatedOn"" DESC;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            location1Filter =req.Level1Filter,
            location2Filter =req.Level2Filter,
            location3Filter =req.Level3Filter,
            location4Filter =req.Level4Filter,
            location5Filter =req.Level5Filter,
            pollingStationNumberFilter = req.PollingStationNumberFilter,
        };

        var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
        var ngoName = multi.Read<string>().SingleOrDefault();
        var notifications = multi.Read<ReceivedNotificationModel>().ToList();

        return TypedResults.Ok(new Response
        {
            NgoName = ngoName,
            Notifications = notifications.ToList()
        });
    }
}
