using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewQueryFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The review and severity queries the endpoints used to build as SQL fragments. The single-select SQL
            // functions are inlined by the planner, so joining them laterally per row costs the same as the fragments did.
            migrationBuilder.Sql(
                """
                -- The NGO's review of a record: its stage (the default one when the NGO hasn't moved the record) and its
                -- reviewers, in assignment order. No row when the NGO doesn't monitor the round.
                CREATE OR REPLACE FUNCTION "GetRecordReviewWorkflow"(
                    electionRoundId UUID,
                    ngoId UUID,
                    recordType TEXT,
                    recordId UUID
                )
                    RETURNS TABLE
                            (
                                "StageId"        UUID,
                                "StageName"      TEXT,
                                "ReviewStatus"   TEXT,
                                "StageChangedAt" TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"     TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"    UUID[]
                            )
                AS
                $$
                SELECT RST."Id",
                       RST."Name"::text,
                       RST."Status",
                       RWF."StageChangedAt",
                       RWF."ReviewedAt",
                       COALESCE((SELECT array_agg(X."UserId" ORDER BY X."AssignedAt")
                                 FROM "Reviewers" X
                                 WHERE X."MonitoringNgoId" = CMN."Id"
                                   AND X."RecordType" = recordType
                                   AND X."RecordId" = recordId), '{}'::uuid[])
                FROM "MonitoringNgos" CMN
                    LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = CMN."Id"
                        AND RWF."RecordType" = recordType
                        AND RWF."RecordId" = recordId
                    INNER JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId",
                        (SELECT D."Id" FROM "ReviewStages" D WHERE D."MonitoringNgoId" = CMN."Id" AND D."IsDefault"))
                WHERE CMN."ElectionRoundId" = electionRoundId
                  AND CMN."NgoId" = ngoId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The NGO's severity of a record, None when it never set one. Always one row.
                CREATE OR REPLACE FUNCTION "GetRecordSeverity"(
                    electionRoundId UUID,
                    ngoId UUID,
                    recordType TEXT,
                    recordId UUID
                )
                    RETURNS TABLE
                            (
                                "Severity" TEXT
                            )
                AS
                $$
                SELECT COALESCE((SELECT RSV."Severity"
                                 FROM "RecordSeverities" RSV
                                     INNER JOIN "MonitoringNgos" SMN ON SMN."Id" = RSV."MonitoringNgoId"
                                 WHERE SMN."ElectionRoundId" = electionRoundId
                                   AND SMN."NgoId" = ngoId
                                   AND RSV."RecordType" = recordType
                                   AND RSV."RecordId" = recordId), 'None')::text
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Orders a severity by level: 1 (None) to 5 (Urgent).
                CREATE OR REPLACE FUNCTION "SeverityRank"(
                    severity TEXT
                )
                    RETURNS INTEGER
                AS
                $$
                SELECT array_position(ARRAY['None', 'Low', 'Medium', 'High', 'Urgent']::text[], severity)
                $$ LANGUAGE sql IMMUTABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The records of a type the NGO can see in a round, using the same rules as the report lists (the
                -- Get*Entries functions with the Coalition data source): form submissions (including polling station
                -- information), quick reports and incident reports from every observer available to the NGO (its own and,
                -- for a coalition leader, the members'); citizen reports from the NGO's own citizen-reporting forms.
                CREATE OR REPLACE FUNCTION "GetVisibleRecordIds"(
                    electionRoundId UUID,
                    ngoId UUID,
                    recordType TEXT
                )
                    RETURNS TABLE
                            (
                                "RecordId" UUID
                            )
                AS
                $$
                SELECT FS."Id"
                FROM "FormSubmissions" FS
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = FS."MonitoringObserverId"
                    INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, 'Coalition') AF ON AF."FormId" = FS."FormId"
                WHERE recordType = 'FormSubmission'
                  AND FS."ElectionRoundId" = electionRoundId
                UNION ALL
                SELECT PSI."Id"
                FROM "PollingStationInformation" PSI
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = PSI."MonitoringObserverId"
                    INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, 'Coalition') AF ON AF."FormId" = PSI."PollingStationInformationFormId"
                WHERE recordType = 'FormSubmission'
                  AND PSI."ElectionRoundId" = electionRoundId
                UNION ALL
                SELECT QR."Id"
                FROM "QuickReports" QR
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = QR."MonitoringObserverId"
                WHERE recordType = 'QuickReport'
                  AND QR."ElectionRoundId" = electionRoundId
                UNION ALL
                SELECT IR."Id"
                FROM "IncidentReports" IR
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = IR."MonitoringObserverId"
                WHERE recordType = 'IncidentReport'
                  AND IR."ElectionRoundId" = electionRoundId
                UNION ALL
                SELECT CR."Id"
                FROM "CitizenReports" CR
                    INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
                WHERE recordType = 'CitizenReport'
                  AND CR."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Counts the records the report lists show the NGO for the data source, per record type, review stage,
                -- severity and (with placeLevel) place. Records the NGO never touched count in its default stage with
                -- severity None. A record has a place at the level when every level down to it is set; the others count
                -- in one row with all levels null. Citizen reports count only when the NGO does citizen reporting in the round.
                CREATE OR REPLACE FUNCTION "GetReviewMetrics"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    placeLevel INTEGER DEFAULT NULL
                )
                    RETURNS TABLE
                            (
                                "RecordType" TEXT,
                                "StageId"    UUID,
                                "Severity"   TEXT,
                                "Level1"     TEXT,
                                "Level2"     TEXT,
                                "Level3"     TEXT,
                                "Level4"     TEXT,
                                "Level5"     TEXT,
                                "Count"      INTEGER
                            )
                AS
                $$
                WITH MN AS (
                    SELECT M."Id" AS "MonitoringNgoId",
                           (SELECT D."Id" FROM "ReviewStages" D WHERE D."MonitoringNgoId" = M."Id" AND D."IsDefault") AS "DefaultStageId"
                    FROM "MonitoringNgos" M
                    WHERE M."ElectionRoundId" = electionRoundId AND M."NgoId" = ngoId
                ), R AS (
                    SELECT 'FormSubmission' AS "RecordType", PSI."Id" AS "RecordId",
                           PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5"
                    FROM "PollingStationInformation" PSI
                        INNER JOIN "PollingStationInformationForms" PSIF ON PSIF."Id" = PSI."PollingStationInformationFormId"
                        INNER JOIN "PollingStations" PS ON PS."Id" = PSI."PollingStationId"
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = PSI."MonitoringObserverId"
                        INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF
                            ON AF."FormId" = PSI."PollingStationInformationFormId"
                    WHERE PSI."ElectionRoundId" = electionRoundId
                    UNION ALL
                    SELECT 'FormSubmission', FS."Id",
                           PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5"
                    FROM "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                        INNER JOIN "PollingStations" PS ON PS."Id" = FS."PollingStationId"
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = FS."MonitoringObserverId"
                        INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF
                            ON AF."FormId" = FS."FormId"
                    WHERE FS."ElectionRoundId" = electionRoundId
                    UNION ALL
                    SELECT 'QuickReport', QR."Id",
                           PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5"
                    FROM "QuickReports" QR
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = QR."MonitoringObserverId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
                    WHERE QR."ElectionRoundId" = electionRoundId
                    UNION ALL
                    SELECT 'IncidentReport', IR."Id",
                           PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5"
                    FROM "IncidentReports" IR
                        INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = IR."MonitoringObserverId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                    WHERE IR."ElectionRoundId" = electionRoundId
                    UNION ALL
                    SELECT 'CitizenReport', CR."Id",
                           L."Level1", L."Level2", L."Level3", L."Level4", L."Level5"
                    FROM "CitizenReports" CR
                        INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                        INNER JOIN "Locations" L ON L."Id" = CR."LocationId"
                        INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                        INNER JOIN "MonitoringNgos" CMN ON CMN."Id" = ER."MonitoringNgoForCitizenReportingId"
                    WHERE CR."ElectionRoundId" = electionRoundId
                      AND CMN."NgoId" = ngoId
                      AND ER."CitizenReportingEnabled" = TRUE
                ), P AS (
                    SELECT R.*,
                           COALESCE(placeLevel, 0) > 0
                               AND (placeLevel < 1 OR NULLIF(R."Level1", '') IS NOT NULL)
                               AND (placeLevel < 2 OR NULLIF(R."Level2", '') IS NOT NULL)
                               AND (placeLevel < 3 OR NULLIF(R."Level3", '') IS NOT NULL)
                               AND (placeLevel < 4 OR NULLIF(R."Level4", '') IS NOT NULL)
                               AND (placeLevel < 5 OR NULLIF(R."Level5", '') IS NOT NULL) AS "HasPlace"
                    FROM R
                ), G AS (
                    SELECT P."RecordType",
                           COALESCE(RWF."StageId", MN."DefaultStageId") AS "StageId",
                           COALESCE(RSV."Severity", 'None')::text AS "Severity",
                           CASE WHEN placeLevel >= 1 AND P."HasPlace" THEN P."Level1" END AS "Level1",
                           CASE WHEN placeLevel >= 2 AND P."HasPlace" THEN P."Level2" END AS "Level2",
                           CASE WHEN placeLevel >= 3 AND P."HasPlace" THEN P."Level3" END AS "Level3",
                           CASE WHEN placeLevel >= 4 AND P."HasPlace" THEN P."Level4" END AS "Level4",
                           CASE WHEN placeLevel >= 5 AND P."HasPlace" THEN P."Level5" END AS "Level5"
                    FROM P
                        CROSS JOIN MN
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = MN."MonitoringNgoId"
                            AND RWF."RecordType" = P."RecordType" AND RWF."RecordId" = P."RecordId"
                        LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = MN."MonitoringNgoId"
                            AND RSV."RecordType" = P."RecordType" AND RSV."RecordId" = P."RecordId"
                )
                SELECT G."RecordType", G."StageId", G."Severity",
                       G."Level1", G."Level2", G."Level3", G."Level4", G."Level5",
                       COUNT(1)::int
                FROM G
                GROUP BY G."RecordType", G."StageId", G."Severity",
                         G."Level1", G."Level2", G."Level3", G."Level4", G."Level5"
                $$ LANGUAGE sql STABLE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetReviewMetrics"(uuid, uuid, text, integer);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetVisibleRecordIds"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "SeverityRank"(text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetRecordSeverity"(uuid, uuid, text, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetRecordReviewWorkflow"(uuid, uuid, text, uuid);""");
        }
    }
}
