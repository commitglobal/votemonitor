using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.PollingStation.Visit.Delete;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, NoContent>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-station-visits/{pollingStationId}");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-visit", "mobile"));
        Summary(s =>
        {
            s.Summary = "Deletes a polling stations visit of an observer";
            s.Description = "Polling station visits are based on polling station information / form answers / notes / attachments/ quick reports/ quick report attachments";
        });
    }

    public override async Task<NoContent> ExecuteAsync(Request req, CancellationToken ct)
    {
        using (var connection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using (var transaction = connection.BeginTransaction())
            {
                string deletePSISubmissionsSql = """
                DELETE FROM "PollingStationInformation"
                WHERE
                    "Id" IN (
                        SELECT
                            PSI."Id"
                        FROM
                            "PollingStationInformation" PSI
                            INNER JOIN "MonitoringObservers" MO ON PSI."MonitoringObserverId" = MO."Id"
                        WHERE
                            PSI."ElectionRoundId" = @electionRoundId
                            AND MO."ObserverId" = @observerId
                            AND PSI."PollingStationId" = @pollingStationId
                    )
                """;

                string deleteFormSubmissionsSql = """
                DELETE FROM "FormSubmissions"
                WHERE
                    "Id" IN (
                        SELECT
                            FS."Id"
                        FROM
                            "FormSubmissions" FS
                            INNER JOIN "MonitoringObservers" MO ON FS."MonitoringObserverId" = MO."Id"
                        WHERE
                            FS."ElectionRoundId" = @electionRoundId
                            AND MO."ObserverId" = @observerId
                            AND FS."PollingStationId" = @pollingStationId
                    )
                """;

                string deleteAttachmentsSql = """
                UPDATE "Attachments"
                SET
                    "IsDeleted" = TRUE
                WHERE
                    "Id" IN (
                        SELECT
                            A."Id"
                        FROM
                            "Attachments" A
                            INNER JOIN "MonitoringObservers" MO ON A."MonitoringObserverId" = MO."Id"
                        WHERE
                            A."ElectionRoundId" = @electionRoundId
                            AND MO."ObserverId" = @observerId
                            AND A."PollingStationId" = @pollingStationId
                    )
                """;

                string deleteNotesSql = """
                DELETE FROM "Notes"
                WHERE
                    "Id" IN (
                        SELECT
                            N."Id"
                        FROM
                            "Notes" N
                            INNER JOIN "MonitoringObservers" MO ON N."MonitoringObserverId" = MO."Id"
                        WHERE
                            N."ElectionRoundId" = @electionRoundId
                            AND MO."ObserverId" = @observerId
                            AND N."PollingStationId" = @pollingStationId
                    )
                """;

                string deleteQuickReportAttachmentsSql = """
                UPDATE "QuickReportAttachments"
                SET
                    "IsDeleted" = TRUE
                WHERE
                    "QuickReportId" IN (
                        SELECT
                            QR."Id"
                        FROM
                            "QuickReports" QR
                            INNER JOIN "MonitoringObservers" MO ON QR."MonitoringObserverId" = MO."Id"
                        WHERE
                            QR."ElectionRoundId" = @electionRoundId
                            AND MO."ObserverId" = @observerId
                            AND QR."PollingStationId" = @pollingStationId
                    )
                """;

                string deleteQuickReportsSql = """
                DELETE FROM "QuickReports"
                WHERE
                    "Id" IN (
                        SELECT
                            QR."Id"
                        FROM
                            "QuickReports" QR
                            INNER JOIN "MonitoringObservers" MO ON QR."MonitoringObserverId" = MO."Id"
                        WHERE
                            QR."ElectionRoundId" = @electionRoundId
                            AND MO."ObserverId" = @observerId
                            AND QR."PollingStationId" = @pollingStationId
                    )
                """;

                var queryParams = new
                {
                    electionRoundId = req.ElectionRoundId,
                    observerId = req.ObserverId,
                    pollingStationId = req.PollingStationId,
                };

                await connection.ExecuteAsync(deletePSISubmissionsSql, queryParams, transaction);
                await connection.ExecuteAsync(deleteFormSubmissionsSql, queryParams, transaction);
                await connection.ExecuteAsync(deleteAttachmentsSql, queryParams, transaction);
                await connection.ExecuteAsync(deleteNotesSql, queryParams, transaction);
                await connection.ExecuteAsync(deleteQuickReportAttachmentsSql, queryParams, transaction);
                await connection.ExecuteAsync(deleteQuickReportsSql, queryParams, transaction);

                transaction.Commit();
            }

            return TypedResults.NoContent();
        }
    }
}
