using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.PollingStation.Visit.ListMy;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-station-visits:my");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-visit", "mobile"));
        Summary(s =>
        {
            s.Summary = "Lists visited polling stations of an observer";
            s.Description = "Polling station visits are based on polling station information / form answers / notes / attachments";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!result.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql ="""
                 SELECT
                     T."PollingStationId",
                     PS."Level1",
                     PS."Level2",
                     PS."Level3",
                     PS."Level4",
                     PS."Level5",
                     PS."Address",
                     PS."Number",
                     T."MonitoringObserverId",
                     MAX(T."LatestTimestamp") "VisitedAt"
                 FROM
                     (
                         SELECT
                             PSI."ElectionRoundId",
                             PSI."PollingStationId",
                             PSI."MonitoringObserverId",
                             COALESCE(PSI."LastModifiedOn", PSI."CreatedOn") "LatestTimestamp"
                         FROM
                             "PollingStationInformation" PSI
                         WHERE
                             PSI."ElectionRoundId" = @electionRoundId
                         UNION
                         SELECT
                             FS."ElectionRoundId",
                             FS."PollingStationId",
                             FS."MonitoringObserverId",
                             COALESCE(FS."LastModifiedOn", FS."CreatedOn") "LatestTimestamp"
                         FROM
                             "FormSubmissions" FS
                         WHERE
                             FS."ElectionRoundId" = @electionRoundId
                         UNION
                         SELECT
                             N."ElectionRoundId",
                             N."PollingStationId",
                             N."MonitoringObserverId",
                             COALESCE(N."LastModifiedOn", N."CreatedOn") "LatestTimestamp"
                         FROM
                             "Notes" N
                         WHERE
                             N."ElectionRoundId" = @electionRoundId
                         UNION
                         SELECT
                             A."ElectionRoundId",
                             A."PollingStationId",
                             A."MonitoringObserverId",
                             COALESCE(A."LastModifiedOn", A."CreatedOn") "LatestTimestamp"
                         FROM
                             "Attachments" A
                         WHERE
                             A."ElectionRoundId" = @electionRoundId
                         UNION
                         SELECT
                             QR."ElectionRoundId",
                             QR."PollingStationId",
                             QR."MonitoringObserverId",
                             COALESCE(QR."LastModifiedOn", QR."CreatedOn") "LatestTimestamp"
                         FROM
                             "QuickReports" QR
                         WHERE
                             QR."ElectionRoundId" = @electionRoundId
                     ) T
                     INNER JOIN "MonitoringObservers" MO ON MO."Id" = T."MonitoringObserverId"
                     INNER JOIN "PollingStations" PS ON PS."Id" = T."PollingStationId"
                 WHERE
                     T."ElectionRoundId" = @electionRoundId
                     AND MO."ObserverId" = @observerId
                 GROUP BY
                     T."ElectionRoundId",
                     T."PollingStationId",
                     T."MonitoringObserverId",
                     PS."Id"
                 """;

        var queryArgs = new {electionRoundId = req.ElectionRoundId,observerId = req.ObserverId };

        IEnumerable<VisitModel> visits = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            visits = await dbConnection.QueryAsync<VisitModel>(sql, queryArgs);
        }

        return TypedResults.Ok(new Response { Visits = visits.ToList() });
    }
}
