using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.PollingStationAttachmentAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;

namespace Feature.PollingStation.Visit.ListMy;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context) : Endpoint<Request, Results<Ok<Response>, NotFound>>
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

        var sql = @$" SELECT t.""ElectionRoundId"",
                     t.""PollingStationId"",
                     ps.""Level1"",
                     ps.""Level2"",
                     ps.""Level3"",
                     ps.""Level4"",
                     ps.""Level5"",
                     ps.""Address"",
                     ps.""Number"",
                     mo.""MonitoringNgoId"",
                     mn.""NgoId"",
                     t.""MonitoringObserverId"",
                     mo.""ObserverId"",
                     MIN(t.""LatestTimestamp"") ""VisitedAt""
                     FROM (
                         SELECT psi.""{nameof(PollingStationInformation.ElectionRoundId)}"",
                         psi.""{nameof(PollingStationInformation.PollingStationId)}"",
                         psi.""{nameof(PollingStationInformation.MonitoringObserverId)}"",
                         COALESCE(psi.""ArrivalTime"", psi.""LastModifiedOn"", psi.""CreatedOn"") ""LatestTimestamp""
                         FROM ""{Tables.PollingStationInformation}"" psi
                         UNION
                         SELECT
                         n.""{nameof(PollingStationNote.ElectionRoundId)}"", 
                         n.""{nameof(PollingStationNote.PollingStationId)}"", 
                         n.""{nameof(PollingStationNote.MonitoringObserverId)}"", 
                         COALESCE(n.""LastModifiedOn"", n.""CreatedOn"") ""LatestTimestamp""
                         FROM ""{Tables.PollingStationNotes}"" n
                         UNION
                         SELECT
                         a.""{nameof(PollingStationAttachment.ElectionRoundId)}"", 
                         a.""{nameof(PollingStationAttachment.PollingStationId)}"", 
                         a.""{nameof(PollingStationNote.MonitoringObserverId)}"", 
                         COALESCE(a.""LastModifiedOn"", a.""CreatedOn"") ""LatestTimestamp""
                         FROM ""{Tables.PollingStationAttachments}"" a
                     ) t 
                     INNER JOIN ""{Tables.MonitoringObservers}"" mo ON mo.""Id"" = t.""MonitoringObserverId""
                     INNER JOIN ""{Tables.MonitoringNgos}"" mn ON mo.""MonitoringNgoId"" = mn.""Id""
                     INNER JOIN ""{Tables.PollingStations}"" ps ON ps.""Id"" = t.""PollingStationId""
                     WHERE t.""ElectionRoundId"" =@electionRoundId AND mo.""ObserverId"" = @observerId
                     GROUP BY 
                           t.""ElectionRoundId"",
                           t.""PollingStationId"",
                           ps.""Level1"",
                           ps.""Level2"",
                           ps.""Level3"",
                           ps.""Level4"",
                           ps.""Level5"",
                           ps.""Address"",
                           ps.""Number"",
                           mo.""MonitoringNgoId"",
                           mn.""NgoId"", 
                           t.""MonitoringObserverId"",
                           mo.""ObserverId"";";
        var queryArgs = new { electionRoundId = req.ElectionRoundId, observerId = req.ObserverId };

        var visits = await context.Connection.QueryAsync<VisitModel>(sql, queryArgs);

        return TypedResults.Ok(new Response { Visits = visits.ToList() });
    }
}
