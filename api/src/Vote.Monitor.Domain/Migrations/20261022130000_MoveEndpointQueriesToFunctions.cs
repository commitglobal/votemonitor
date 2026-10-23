using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class MoveEndpointQueriesToFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The select queries the endpoints and export jobs used to build inline, one function per query. Endpoints
            // keep only runtime-built parts: sorting and paging over the function, and FilterSqlCompiler filters.
            // statistics-a
            migrationBuilder.Sql(
                """
                -- Name and number of members of a coalition in an election round; no row when it does not exist.
                CREATE OR REPLACE FUNCTION "StatsOverviewCoalitionHeader"(electionRoundId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "Name" TEXT,
                            "NumberOfMembers" INTEGER
                        )
                AS
                $$
                SELECT
                    C."Name"::text,
                    (SELECT COUNT(*) FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = C."Id")::INT AS "NumberOfMembers"
                FROM "Coalitions" C
                WHERE C."Id" = coalitionId
                  AND C."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Status of a monitoring NGO in an election round; no row when it does not exist.
                CREATE OR REPLACE FUNCTION "StatsOverviewMonitoringNgoStatus"(electionRoundId UUID, monitoringNgoId UUID)
                    RETURNS TABLE
                        (
                            "Status" TEXT
                        )
                AS
                $$
                SELECT MN."Status"
                FROM "MonitoringNgos" MN
                WHERE MN."Id" = monitoringNgoId
                  AND MN."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of active and suspended monitoring NGOs in an election round.
                CREATE OR REPLACE FUNCTION "StatsOverviewNgosCounts"(electionRoundId UUID)
                    RETURNS TABLE
                        (
                            "ActiveNgos" BIGINT,
                            "InactiveNgos" BIGINT
                        )
                AS
                $$
                SELECT
                    COUNT(*) FILTER (WHERE MN."Status" = 'Active') AS "ActiveNgos",
                    COUNT(*) FILTER (WHERE MN."Status" = 'Suspended') AS "InactiveNgos"
                FROM "MonitoringNgos" MN
                WHERE MN."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of coalitions in an election round.
                CREATE OR REPLACE FUNCTION "StatsOverviewCoalitionsCount"(electionRoundId UUID)
                    RETURNS INTEGER
                AS
                $$
                SELECT COUNT(*)::INT
                FROM "Coalitions" C
                WHERE C."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Active, pending and suspended monitoring observers.
                -- Scope: the given monitoring NGO, the given coalition's members, or every NGO in the round when both are NULL.
                CREATE OR REPLACE FUNCTION "StatsOverviewObservers"(electionRoundId UUID, monitoringNgoId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "ActiveObservers" BIGINT,
                            "PendingObservers" BIGINT,
                            "SuspendedObservers" BIGINT
                        )
                AS
                $$
                SELECT
                    COUNT(MO."Id") FILTER (
                        WHERE MO."Status" = 'Active'
                          AND U."Status" = 'Active'
                    ) AS "ActiveObservers",
                    COUNT(MO."Id") FILTER (
                        WHERE MO."Status" = 'Pending'
                          OR U."Status" = 'Pending'
                    ) AS "PendingObservers",
                    COUNT(MO."Id") FILTER (
                        WHERE MO."Status" = 'Suspended'
                          OR U."Status" = 'Deactivated'
                    ) AS "SuspendedObservers"
                FROM "MonitoringObservers" MO
                    INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                WHERE MO."ElectionRoundId" = electionRoundId
                  AND (monitoringNgoId IS NULL OR MO."MonitoringNgoId" = monitoringNgoId)
                  AND (coalitionId IS NULL OR MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = coalitionId AND CM."ElectionRoundId" = electionRoundId))
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Visited polling stations, reports, minutes monitoring, active observers and coverage per polling station level path (level 0 is the total).
                -- Scope: the given monitoring NGO, the given coalition's members, or every NGO in the round when both are NULL.
                CREATE OR REPLACE FUNCTION "StatsOverviewPollingStationLevels"(electionRoundId UUID, monitoringNgoId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "Path" TEXT,
                            "Level" INTEGER,
                            "NumberOfVisitedPollingStations" BIGINT,
                            "NumberOfPollingStations" BIGINT,
                            "NumberOfIncidentReports" NUMERIC,
                            "NumberOfQuickReports" NUMERIC,
                            "NumberOfFormSubmissions" NUMERIC,
                            "MinutesMonitoring" DOUBLE PRECISION,
                            "NumberOfFlaggedAnswers" NUMERIC,
                            "NumberOfQuestionsAnswered" NUMERIC,
                            "ActiveObservers" BIGINT,
                            "CoveragePercentage" NUMERIC
                        )
                AS
                $$
                WITH
                    "AvailableObservers" AS (
                        SELECT MO."Id" AS "MonitoringObserverId"
                        FROM "MonitoringObservers" MO
                        WHERE MO."ElectionRoundId" = electionRoundId
                          AND (monitoringNgoId IS NULL OR MO."MonitoringNgoId" = monitoringNgoId)
                          AND (coalitionId IS NULL OR MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = coalitionId AND CM."ElectionRoundId" = electionRoundId))
                    ),
                    "ActiveObservers" AS (
                        SELECT
                            FS."PollingStationId",
                            FS."MonitoringObserverId"
                        FROM "FormSubmissions" FS
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                        UNION
                        SELECT
                            PSI."PollingStationId",
                            PSI."MonitoringObserverId"
                        FROM "PollingStationInformation" PSI
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                        UNION
                        SELECT
                            QR."PollingStationId",
                            QR."MonitoringObserverId"
                        FROM "QuickReports" QR
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                        WHERE QR."PollingStationId" IS NOT NULL
                        UNION
                        SELECT
                            IR."PollingStationId",
                            IR."MonitoringObserverId"
                        FROM "IncidentReports" IR
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                        WHERE IR."PollingStationId" IS NOT NULL
                    ),
                    "PollingStationsStatsRaw" AS (
                        SELECT
                            FS."PollingStationId",
                            0 AS "NumberOfIncidentReports",
                            0 AS "NumberOfQuickReports",
                            COUNT(1) AS "NumberOfFormSubmissions",
                            0::float AS "MinutesMonitoring",
                            SUM(FS."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                            SUM(FS."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                        FROM "FormSubmissions" FS
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                        GROUP BY FS."PollingStationId"
                        UNION ALL
                        SELECT
                            PSI."PollingStationId",
                            0 AS "NumberOfIncidentReports",
                            0 AS "NumberOfQuickReports",
                            COUNT(1) AS "NumberOfFormSubmissions",
                            SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks")) AS "MinutesMonitoring",
                            SUM(PSI."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                            SUM(PSI."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                        FROM "PollingStationInformation" PSI
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                        GROUP BY PSI."PollingStationId"
                        UNION ALL
                        SELECT
                            QR."PollingStationId",
                            0 AS "NumberOfIncidentReports",
                            COUNT(1) AS "NumberOfQuickReports",
                            0 AS "NumberOfFormSubmissions",
                            0::float AS "MinutesMonitoring",
                            0 AS "NumberOfFlaggedAnswers",
                            0 AS "NumberOfQuestionsAnswered"
                        FROM "QuickReports" QR
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                        WHERE QR."PollingStationId" IS NOT NULL
                        GROUP BY QR."PollingStationId"
                        UNION ALL
                        SELECT
                            IR."PollingStationId",
                            COUNT(1) AS "NumberOfIncidentReports",
                            0 AS "NumberOfQuickReports",
                            0 AS "NumberOfFormSubmissions",
                            0::float AS "MinutesMonitoring",
                            0 AS "NumberOfFlaggedAnswers",
                            0 AS "NumberOfQuestionsAnswered"
                        FROM "IncidentReports" IR
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                        WHERE IR."PollingStationId" IS NOT NULL
                        GROUP BY IR."PollingStationId"
                    ),
                    "PollingStationsStats" AS (
                        SELECT
                            "PollingStationId",
                            SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                            SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                            SUM("NumberOfFormSubmissions") AS "NumberOfFormSubmissions",
                            SUM("MinutesMonitoring") AS "MinutesMonitoring",
                            SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                            SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                        FROM "PollingStationsStatsRaw"
                        GROUP BY "PollingStationId"
                    ),
                    "ElectionPollingStations" AS (
                        SELECT
                            PS."Id",
                            NULLIF(PS."Level1", '') AS "Level1",
                            NULLIF(PS."Level2", '') AS "Level2",
                            NULLIF(PS."Level3", '') AS "Level3",
                            NULLIF(PS."Level4", '') AS "Level4",
                            NULLIF(PS."Level5", '') AS "Level5"
                        FROM "PollingStations" PS
                        WHERE PS."ElectionRoundId" = electionRoundId
                          AND PS."Level1" != ''
                    ),
                    "PollingStationsPerLevel" AS (
                        SELECT
                            CASE
                                WHEN GROUPING(PS."Level1") = 1 THEN 0
                                WHEN GROUPING(PS."Level2") = 1 THEN 1
                                WHEN GROUPING(PS."Level3") = 1 THEN 2
                                WHEN GROUPING(PS."Level4") = 1 THEN 3
                                WHEN GROUPING(PS."Level5") = 1 THEN 4
                                ELSE 5
                            END AS "Level",
                            CASE
                                WHEN GROUPING(PS."Level1") = 1 THEN '/'
                                ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                            END AS "Path",
                            COUNT(PS."Id") AS "NumberOfPollingStations"
                        FROM "ElectionPollingStations" PS
                        GROUP BY GROUPING SETS (
                            (),
                            (PS."Level1"),
                            (PS."Level1", PS."Level2"),
                            (PS."Level1", PS."Level2", PS."Level3"),
                            (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                            (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                        )
                        HAVING
                            GROUPING(PS."Level1") = 1
                            OR (
                                GROUPING(PS."Level2") = 1
                                AND PS."Level1" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level3") = 1
                                AND GROUPING(PS."Level2") = 0
                                AND PS."Level2" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level4") = 1
                                AND GROUPING(PS."Level3") = 0
                                AND PS."Level3" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level5") = 1
                                AND GROUPING(PS."Level4") = 0
                                AND PS."Level4" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level5") = 0
                                AND PS."Level5" IS NOT NULL
                            )
                    ),
                    "PollingStationLevelsStats" AS (
                        SELECT
                            CASE
                                WHEN GROUPING(PS."Level1") = 1 THEN 0
                                WHEN GROUPING(PS."Level2") = 1 THEN 1
                                WHEN GROUPING(PS."Level3") = 1 THEN 2
                                WHEN GROUPING(PS."Level4") = 1 THEN 3
                                WHEN GROUPING(PS."Level5") = 1 THEN 4
                                ELSE 5
                            END AS "Level",
                            CASE
                                WHEN GROUPING(PS."Level1") = 1 THEN '/'
                                ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                            END AS "Path",
                            COUNT(PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                            COALESCE(SUM(PSV."NumberOfIncidentReports"), 0) AS "NumberOfIncidentReports",
                            COALESCE(SUM(PSV."NumberOfQuickReports"), 0) AS "NumberOfQuickReports",
                            COALESCE(SUM(PSV."NumberOfFormSubmissions"), 0) AS "NumberOfFormSubmissions",
                            COALESCE(SUM(PSV."MinutesMonitoring"), 0) AS "MinutesMonitoring",
                            COALESCE(SUM(PSV."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                            COALESCE(SUM(PSV."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered"
                        FROM "PollingStationsStats" PSV
                            INNER JOIN "ElectionPollingStations" PS ON PS."Id" = PSV."PollingStationId"
                        GROUP BY GROUPING SETS (
                            (),
                            (PS."Level1"),
                            (PS."Level1", PS."Level2"),
                            (PS."Level1", PS."Level2", PS."Level3"),
                            (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                            (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                        )
                        HAVING
                            GROUPING(PS."Level1") = 1
                            OR (
                                GROUPING(PS."Level2") = 1
                                AND PS."Level1" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level3") = 1
                                AND GROUPING(PS."Level2") = 0
                                AND PS."Level2" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level4") = 1
                                AND GROUPING(PS."Level3") = 0
                                AND PS."Level3" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level5") = 1
                                AND GROUPING(PS."Level4") = 0
                                AND PS."Level4" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level5") = 0
                                AND PS."Level5" IS NOT NULL
                            )
                    ),
                    "ActiveObserversPerLevel" AS (
                        SELECT
                            CASE
                                WHEN GROUPING(PS."Level1") = 1 THEN 0
                                WHEN GROUPING(PS."Level2") = 1 THEN 1
                                WHEN GROUPING(PS."Level3") = 1 THEN 2
                                WHEN GROUPING(PS."Level4") = 1 THEN 3
                                WHEN GROUPING(PS."Level5") = 1 THEN 4
                                ELSE 5
                            END AS "Level",
                            CASE
                                WHEN GROUPING(PS."Level1") = 1 THEN '/'
                                ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                            END AS "Path",
                            COUNT(DISTINCT AO."MonitoringObserverId") AS "ActiveObservers"
                        FROM "ActiveObservers" AO
                            INNER JOIN "ElectionPollingStations" PS ON PS."Id" = AO."PollingStationId"
                        GROUP BY GROUPING SETS (
                            (),
                            (PS."Level1"),
                            (PS."Level1", PS."Level2"),
                            (PS."Level1", PS."Level2", PS."Level3"),
                            (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                            (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                        )
                        HAVING
                            GROUPING(PS."Level1") = 1
                            OR (
                                GROUPING(PS."Level2") = 1
                                AND PS."Level1" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level3") = 1
                                AND GROUPING(PS."Level2") = 0
                                AND PS."Level2" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level4") = 1
                                AND GROUPING(PS."Level3") = 0
                                AND PS."Level3" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level5") = 1
                                AND GROUPING(PS."Level4") = 0
                                AND PS."Level4" IS NOT NULL
                            )
                            OR (
                                GROUPING(PS."Level5") = 0
                                AND PS."Level5" IS NOT NULL
                            )
                    )
                SELECT
                    S."Path",
                    S."Level",
                    S."NumberOfVisitedPollingStations",
                    PS."NumberOfPollingStations",
                    S."NumberOfIncidentReports",
                    S."NumberOfQuickReports",
                    S."NumberOfFormSubmissions",
                    S."MinutesMonitoring",
                    S."NumberOfFlaggedAnswers",
                    S."NumberOfQuestionsAnswered",
                    AOL."ActiveObservers",
                    (
                        S."NumberOfVisitedPollingStations" * 100.0 / NULLIF(PS."NumberOfPollingStations", 0)
                    ) AS "CoveragePercentage"
                FROM "PollingStationLevelsStats" S
                    INNER JOIN "PollingStationsPerLevel" PS ON S."Level" = PS."Level" AND S."Path" = PS."Path"
                    INNER JOIN "ActiveObserversPerLevel" AOL ON S."Level" = AOL."Level" AND S."Path" = AOL."Path"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Visited locations, citizen reports and coverage per location level path (level 0 is the total).
                -- Scope: the given monitoring NGO, the given coalition's members, or every NGO in the round when both are NULL.
                CREATE OR REPLACE FUNCTION "StatsOverviewLocationLevels"(electionRoundId UUID, monitoringNgoId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "Path" TEXT,
                            "Level" INTEGER,
                            "NumberOfVisitedLocations" BIGINT,
                            "NumberOfLocations" BIGINT,
                            "NumberOfCitizenReports" NUMERIC,
                            "NumberOfFlaggedAnswers" NUMERIC,
                            "NumberOfQuestionsAnswered" NUMERIC,
                            "CoveragePercentage" NUMERIC
                        )
                AS
                $$
                WITH
                    "LocationStatsRaw" AS (
                        SELECT
                            CR."LocationId",
                            COUNT(1) AS "NumberOfCitizenReports",
                            SUM(CR."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                            SUM(CR."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                        FROM "CitizenReports" CR
                            INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                        WHERE CR."ElectionRoundId" = electionRoundId
                          AND (monitoringNgoId IS NULL OR ER."MonitoringNgoForCitizenReportingId" = monitoringNgoId)
                          AND (coalitionId IS NULL OR ER."MonitoringNgoForCitizenReportingId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = coalitionId AND CM."ElectionRoundId" = electionRoundId))
                          AND CR."NumberOfQuestionsAnswered" > 0
                        GROUP BY CR."LocationId"
                    ),
                    "ElectionLocations" AS (
                        SELECT
                            L."Id",
                            NULLIF(L."Level1", '') AS "Level1",
                            NULLIF(L."Level2", '') AS "Level2",
                            NULLIF(L."Level3", '') AS "Level3",
                            NULLIF(L."Level4", '') AS "Level4",
                            NULLIF(L."Level5", '') AS "Level5"
                        FROM "Locations" L
                        WHERE L."ElectionRoundId" = electionRoundId
                          AND L."Level1" != ''
                    ),
                    "LocationsPerLevel" AS (
                        SELECT
                            CASE
                                WHEN GROUPING(L."Level1") = 1 THEN 0
                                WHEN GROUPING(L."Level2") = 1 THEN 1
                                WHEN GROUPING(L."Level3") = 1 THEN 2
                                WHEN GROUPING(L."Level4") = 1 THEN 3
                                WHEN GROUPING(L."Level5") = 1 THEN 4
                                ELSE 5
                            END AS "Level",
                            CASE
                                WHEN GROUPING(L."Level1") = 1 THEN '/'
                                ELSE CONCAT_WS(' / ', L."Level1", L."Level2", L."Level3", L."Level4", L."Level5")
                            END AS "Path",
                            COUNT(L."Id") AS "NumberOfLocations"
                        FROM "ElectionLocations" L
                        GROUP BY GROUPING SETS (
                            (),
                            (L."Level1"),
                            (L."Level1", L."Level2"),
                            (L."Level1", L."Level2", L."Level3"),
                            (L."Level1", L."Level2", L."Level3", L."Level4"),
                            (L."Level1", L."Level2", L."Level3", L."Level4", L."Level5")
                        )
                        HAVING
                            GROUPING(L."Level1") = 1
                            OR (
                                GROUPING(L."Level2") = 1
                                AND L."Level1" IS NOT NULL
                            )
                            OR (
                                GROUPING(L."Level3") = 1
                                AND GROUPING(L."Level2") = 0
                                AND L."Level2" IS NOT NULL
                            )
                            OR (
                                GROUPING(L."Level4") = 1
                                AND GROUPING(L."Level3") = 0
                                AND L."Level3" IS NOT NULL
                            )
                            OR (
                                GROUPING(L."Level5") = 1
                                AND GROUPING(L."Level4") = 0
                                AND L."Level4" IS NOT NULL
                            )
                            OR (
                                GROUPING(L."Level5") = 0
                                AND L."Level5" IS NOT NULL
                            )
                    ),
                    "LocationLevelsStats" AS (
                        SELECT
                            CASE
                                WHEN GROUPING(L."Level1") = 1 THEN 0
                                WHEN GROUPING(L."Level2") = 1 THEN 1
                                WHEN GROUPING(L."Level3") = 1 THEN 2
                                WHEN GROUPING(L."Level4") = 1 THEN 3
                                WHEN GROUPING(L."Level5") = 1 THEN 4
                                ELSE 5
                            END AS "Level",
                            CASE
                                WHEN GROUPING(L."Level1") = 1 THEN '/'
                                ELSE CONCAT_WS(' / ', L."Level1", L."Level2", L."Level3", L."Level4", L."Level5")
                            END AS "Path",
                            COUNT(LS."LocationId") AS "NumberOfVisitedLocations",
                            COALESCE(SUM(LS."NumberOfCitizenReports"), 0) AS "NumberOfCitizenReports",
                            COALESCE(SUM(LS."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                            COALESCE(SUM(LS."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered"
                        FROM "LocationStatsRaw" LS
                            INNER JOIN "ElectionLocations" L ON L."Id" = LS."LocationId"
                        GROUP BY GROUPING SETS (
                            (),
                            (L."Level1"),
                            (L."Level1", L."Level2"),
                            (L."Level1", L."Level2", L."Level3"),
                            (L."Level1", L."Level2", L."Level3", L."Level4"),
                            (L."Level1", L."Level2", L."Level3", L."Level4", L."Level5")
                        )
                        HAVING
                            GROUPING(L."Level1") = 1
                            OR (
                                GROUPING(L."Level2") = 1
                                AND L."Level1" IS NOT NULL
                            )
                            OR (
                                GROUPING(L."Level3") = 1
                                AND GROUPING(L."Level2") = 0
                                AND L."Level2" IS NOT NULL
                            )
                            OR (
                                GROUPING(L."Level4") = 1
                                AND GROUPING(L."Level3") = 0
                                AND L."Level3" IS NOT NULL
                            )
                            OR (
                                GROUPING(L."Level5") = 1
                                AND GROUPING(L."Level4") = 0
                                AND L."Level4" IS NOT NULL
                            )
                            OR (
                                GROUPING(L."Level5") = 0
                                AND L."Level5" IS NOT NULL
                            )
                    )
                SELECT
                    S."Path",
                    S."Level",
                    S."NumberOfVisitedLocations",
                    L."NumberOfLocations",
                    S."NumberOfCitizenReports",
                    S."NumberOfFlaggedAnswers",
                    S."NumberOfQuestionsAnswered",
                    (
                        S."NumberOfVisitedLocations" * 100.0 / NULLIF(L."NumberOfLocations", 0)
                    ) AS "CoveragePercentage"
                FROM "LocationLevelsStats" S
                    INNER JOIN "LocationsPerLevel" L ON S."Level" = L."Level" AND S."Path" = L."Path"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Form submissions and polling station information per hour, with answered questions and flagged answers, ordered by bucket.
                -- Scope: the given monitoring NGO, the given coalition's members, or every NGO in the round when both are NULL.
                CREATE OR REPLACE FUNCTION "StatsOverviewFormsHistogram"(electionRoundId UUID, monitoringNgoId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "Bucket" TIMESTAMP WITH TIME ZONE,
                            "FormsSubmitted" BIGINT,
                            "NumberOfQuestionsAnswered" BIGINT,
                            "NumberOfFlaggedAnswers" BIGINT
                        )
                AS
                $$
                WITH "AvailableObservers" AS (
                    SELECT MO."Id" AS "MonitoringObserverId"
                    FROM "MonitoringObservers" MO
                    WHERE MO."ElectionRoundId" = electionRoundId
                      AND (monitoringNgoId IS NULL OR MO."MonitoringNgoId" = monitoringNgoId)
                      AND (coalitionId IS NULL OR MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = coalitionId AND CM."ElectionRoundId" = electionRoundId))
                ),
                submission_histogram AS (
                    SELECT
                        DATE_TRUNC(
                                'hour',
                                TIMEZONE('utc', FS."LastUpdatedAt")
                        )::timestamptz AS bucket,
                        1 AS forms_submitted,
                        FS."NumberOfQuestionsAnswered" AS number_of_questions_answered,
                        FS."NumberOfFlaggedAnswers" AS number_of_flagged_answers
                    FROM "FormSubmissions" FS
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"

                    UNION ALL

                    SELECT
                        DATE_TRUNC(
                                'hour',
                                TIMEZONE('utc', qr."LastUpdatedAt")
                        )::timestamptz AS bucket,
                        1 AS forms_submitted,
                        qr."NumberOfQuestionsAnswered" AS number_of_questions_answered,
                        qr."NumberOfFlaggedAnswers" AS number_of_flagged_answers
                    FROM "PollingStationInformation" qr
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = qr."MonitoringObserverId"
                )
                SELECT
                    bucket AS "Bucket",
                    COUNT(*) AS "FormsSubmitted",
                    SUM(number_of_questions_answered) AS "NumberOfQuestionsAnswered",
                    SUM(number_of_flagged_answers) AS "NumberOfFlaggedAnswers"
                FROM submission_histogram
                GROUP BY bucket
                ORDER BY bucket
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Answers and flagged answers per hour of form submissions and polling station information, ordered by bucket.
                -- Scope: the given monitoring NGO, the given coalition's members, or every NGO in the round when both are NULL.
                CREATE OR REPLACE FUNCTION "StatsOverviewAnswersHistogram"(electionRoundId UUID, monitoringNgoId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "Bucket" TIMESTAMP WITH TIME ZONE,
                            "NumberOfAnswers" BIGINT,
                            "NumberOfFlaggedAnswers" NUMERIC
                        )
                AS
                $$
                WITH "AvailableObservers" AS (
                    SELECT MO."Id" AS "MonitoringObserverId"
                    FROM "MonitoringObservers" MO
                    WHERE MO."ElectionRoundId" = electionRoundId
                      AND (monitoringNgoId IS NULL OR MO."MonitoringNgoId" = monitoringNgoId)
                      AND (coalitionId IS NULL OR MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = coalitionId AND CM."ElectionRoundId" = electionRoundId))
                ),
                "Submissions" AS (
                    SELECT FS."Answers", F."Questions", FS."LastUpdatedAt", FS."CreatedAt"
                    FROM "FormSubmissions" FS
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"

                    UNION ALL

                    SELECT PSI."Answers", PSIF."Questions", PSI."LastUpdatedAt", NULL::timestamptz AS "CreatedAt"
                    FROM "PollingStationInformation" PSI
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                        INNER JOIN "PollingStationInformationForms" PSIF ON PSIF."Id" = PSI."PollingStationInformationFormId"
                ),
                -- answers to the form's questions, timed by the answer's own timestamps when the client sent them
                "SubmissionAnswers" AS (
                    SELECT
                        COALESCE(
                            (A ->> 'LastUpdatedAt')::timestamptz,
                            (A ->> 'CreatedAt')::timestamptz,
                            S."LastUpdatedAt",
                            S."CreatedAt"
                        ) AS answered_at,
                        -- flagged options selected in a single or multi select answer
                        CASE
                            WHEN A ->> '$answerType' = 'singleSelectAnswer' AND Q ->> '$questionType' = 'singleSelectQuestion'
                                THEN (SELECT COUNT(*)
                                      FROM JSONB_ARRAY_ELEMENTS(Q -> 'Options') O
                                      WHERE (O ->> 'IsFlagged')::boolean
                                        AND O ->> 'Id' = A -> 'Selection' ->> 'OptionId')
                            WHEN A ->> '$answerType' = 'multiSelectAnswer' AND Q ->> '$questionType' = 'multiSelectQuestion'
                                THEN (SELECT COUNT(*)
                                      FROM JSONB_ARRAY_ELEMENTS(Q -> 'Options') O
                                      WHERE (O ->> 'IsFlagged')::boolean
                                        AND EXISTS (SELECT 1
                                                    FROM JSONB_ARRAY_ELEMENTS(A -> 'Selection') SEL
                                                    WHERE SEL ->> 'OptionId' = O ->> 'Id'))
                            ELSE 0
                        END AS number_of_flagged_answers
                    FROM "Submissions" S
                        CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Answers") A
                        INNER JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Questions") Q ON Q ->> 'Id' = A ->> 'QuestionId'
                )
                SELECT
                    DATE_TRUNC(
                            'hour',
                            TIMEZONE('utc', answered_at)
                    )::timestamptz AS "Bucket",
                    COUNT(*) AS "NumberOfAnswers",
                    SUM(number_of_flagged_answers) AS "NumberOfFlaggedAnswers"
                FROM "SubmissionAnswers"
                GROUP BY 1
                ORDER BY 1
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Quick reports per hour.
                -- Scope: the given monitoring NGO, the given coalition's members, or every NGO in the round when both are NULL.
                CREATE OR REPLACE FUNCTION "StatsOverviewQuickReportsHistogram"(electionRoundId UUID, monitoringNgoId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "Bucket" TIMESTAMP WITH TIME ZONE,
                            "Value" BIGINT
                        )
                AS
                $$
                WITH "AvailableObservers" AS (
                    SELECT MO."Id" AS "MonitoringObserverId"
                    FROM "MonitoringObservers" MO
                    WHERE MO."ElectionRoundId" = electionRoundId
                      AND (monitoringNgoId IS NULL OR MO."MonitoringNgoId" = monitoringNgoId)
                      AND (coalitionId IS NULL OR MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = coalitionId AND CM."ElectionRoundId" = electionRoundId))
                )
                SELECT
                    DATE_TRUNC(
                            'hour',
                            TIMEZONE('utc', QR."LastUpdatedAt")
                    )::TIMESTAMPTZ "Bucket",
                    COUNT(1) "Value"
                FROM "QuickReports" QR
                    INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                GROUP BY 1
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Citizen reports per hour.
                -- Scope: the given monitoring NGO, the given coalition's members, or every NGO in the round when both are NULL.
                CREATE OR REPLACE FUNCTION "StatsOverviewCitizenReportsHistogram"(electionRoundId UUID, monitoringNgoId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "Bucket" TIMESTAMP WITH TIME ZONE,
                            "Value" BIGINT
                        )
                AS
                $$
                SELECT
                    DATE_TRUNC(
                            'hour',
                            TIMEZONE(
                                    'utc',
                                    COALESCE(CR."LastModifiedOn", CR."CreatedOn")
                            )
                    )::TIMESTAMPTZ "Bucket",
                    COUNT(1) "Value"
                FROM "CitizenReports" CR
                    INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                WHERE CR."ElectionRoundId" = electionRoundId
                  AND (monitoringNgoId IS NULL OR ER."MonitoringNgoForCitizenReportingId" = monitoringNgoId)
                  AND (coalitionId IS NULL OR ER."MonitoringNgoForCitizenReportingId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = coalitionId AND CM."ElectionRoundId" = electionRoundId))
                GROUP BY 1
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Incident reports per hour.
                -- Scope: the given monitoring NGO, the given coalition's members, or every NGO in the round when both are NULL.
                CREATE OR REPLACE FUNCTION "StatsOverviewIncidentReportsHistogram"(electionRoundId UUID, monitoringNgoId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "Bucket" TIMESTAMP WITH TIME ZONE,
                            "Value" BIGINT
                        )
                AS
                $$
                WITH "AvailableObservers" AS (
                    SELECT MO."Id" AS "MonitoringObserverId"
                    FROM "MonitoringObservers" MO
                    WHERE MO."ElectionRoundId" = electionRoundId
                      AND (monitoringNgoId IS NULL OR MO."MonitoringNgoId" = monitoringNgoId)
                      AND (coalitionId IS NULL OR MO."MonitoringNgoId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = coalitionId AND CM."ElectionRoundId" = electionRoundId))
                )
                SELECT
                    DATE_TRUNC(
                            'hour',
                            TIMEZONE('utc', IR."LastUpdatedAt")
                    )::TIMESTAMPTZ "Bucket",
                    COUNT(1) "Value"
                FROM "IncidentReports" IR
                    INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                GROUP BY 1
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Citizen report submissions, answers and flagged answers per hour, ordered by bucket.
                -- Scope: the given monitoring NGO, the given coalition's members, or every NGO in the round when both are NULL.
                CREATE OR REPLACE FUNCTION "StatsOverviewCitizenReportAnswersHistogram"(electionRoundId UUID, monitoringNgoId UUID, coalitionId UUID)
                    RETURNS TABLE
                        (
                            "Bucket" TIMESTAMP WITH TIME ZONE,
                            "NumberOfSubmissions" BIGINT,
                            "NumberOfAnswers" BIGINT,
                            "NumberOfFlaggedAnswers" NUMERIC
                        )
                AS
                $$
                WITH "CitizenReportSubmissions" AS (
                    SELECT CR."Answers", F."Questions", CR."CreatedOn" AS "CreatedAt"
                    FROM "CitizenReports" CR
                        INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                        INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                    WHERE CR."ElectionRoundId" = electionRoundId
                      AND (monitoringNgoId IS NULL OR ER."MonitoringNgoForCitizenReportingId" = monitoringNgoId)
                      AND (coalitionId IS NULL OR ER."MonitoringNgoForCitizenReportingId" IN (SELECT CM."MonitoringNgoId" FROM "CoalitionMemberships" CM WHERE CM."CoalitionId" = coalitionId AND CM."ElectionRoundId" = electionRoundId))
                ),
                -- answers to the form's questions
                "SubmissionAnswers" AS (
                    SELECT
                        -- citizens' devices are not trusted: use the time the report was received
                        S."CreatedAt" AS answered_at,
                        -- flagged options selected in a single or multi select answer
                        CASE
                            WHEN A ->> '$answerType' = 'singleSelectAnswer' AND Q ->> '$questionType' = 'singleSelectQuestion'
                                THEN (SELECT COUNT(*)
                                      FROM JSONB_ARRAY_ELEMENTS(Q -> 'Options') O
                                      WHERE (O ->> 'IsFlagged')::boolean
                                        AND O ->> 'Id' = A -> 'Selection' ->> 'OptionId')
                            WHEN A ->> '$answerType' = 'multiSelectAnswer' AND Q ->> '$questionType' = 'multiSelectQuestion'
                                THEN (SELECT COUNT(*)
                                      FROM JSONB_ARRAY_ELEMENTS(Q -> 'Options') O
                                      WHERE (O ->> 'IsFlagged')::boolean
                                        AND EXISTS (SELECT 1
                                                    FROM JSONB_ARRAY_ELEMENTS(A -> 'Selection') SEL
                                                    WHERE SEL ->> 'OptionId' = O ->> 'Id'))
                            ELSE 0
                        END AS number_of_flagged_answers
                    FROM "CitizenReportSubmissions" S
                        CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Answers") A
                        INNER JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Questions") Q ON Q ->> 'Id' = A ->> 'QuestionId'
                ),
                "CitizenReportsHistogram" AS (
                    SELECT S."CreatedAt" AS received_at, 1 AS submissions, 0 AS answers, 0 AS flagged_answers
                    FROM "CitizenReportSubmissions" S

                    UNION ALL

                    SELECT answered_at, 0, 1, number_of_flagged_answers
                    FROM "SubmissionAnswers"
                )
                SELECT
                    DATE_TRUNC(
                            'hour',
                            TIMEZONE('utc', received_at)
                    )::timestamptz AS "Bucket",
                    SUM(submissions) AS "NumberOfSubmissions",
                    SUM(answers) AS "NumberOfAnswers",
                    SUM(flagged_answers) AS "NumberOfFlaggedAnswers"
                FROM "CitizenReportsHistogram"
                GROUP BY 1
                ORDER BY 1
                $$ LANGUAGE sql STABLE;
                """);

            // statistics-b
            migrationBuilder.Sql(
                """
                -- Totals (submissions, answers, flagged answers, observers, polling stations) for one form, scoped to the observers available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetFormStatisticsTotals"(electionRoundId UUID, ngoId UUID, formId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "NumberOfSubmissions" BIGINT,
                    "NumberOfQuestionsAnswered" BIGINT,
                    "NumberOfFlaggedAnswers" BIGINT,
                    "NumberOfObservers" BIGINT,
                    "NumberOfPollingStations" BIGINT
                )
                AS $$
                    SELECT
                        COUNT(*) AS "NumberOfSubmissions",
                        COALESCE(SUM(FS."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered",
                        COALESCE(SUM(FS."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                        COUNT(DISTINCT FS."MonitoringObserverId") AS "NumberOfObservers",
                        COUNT(DISTINCT FS."PollingStationId") AS "NumberOfPollingStations"
                    FROM "FormSubmissions" FS
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                        ON AMO."MonitoringObserverId" = FS."MonitoringObserverId"
                    WHERE FS."ElectionRoundId" = electionRoundId
                      AND FS."FormId" = formId;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Totals (form submissions, answers, quick reports, observers, PSI presence) for one polling station, scoped to the observers available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetPollingStationStatisticsTotals"(electionRoundId UUID, ngoId UUID, pollingStationId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "NumberOfFormSubmissions" BIGINT,
                    "NumberOfQuestionsAnswered" BIGINT,
                    "NumberOfQuickReports" BIGINT,
                    "NumberOfObservers" BIGINT,
                    "HasPollingStationInformation" BOOLEAN
                )
                AS $$
                    WITH "AvailableObservers" AS (
                        SELECT "MonitoringObserverId"
                        FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource)
                    )
                    SELECT
                        (
                            SELECT COUNT(*)
                            FROM "FormSubmissions" FS
                            WHERE FS."ElectionRoundId" = electionRoundId
                              AND FS."PollingStationId" = pollingStationId
                              AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        ) AS "NumberOfFormSubmissions",
                        (
                            SELECT COALESCE(SUM(FS."NumberOfQuestionsAnswered"), 0)
                            FROM "FormSubmissions" FS
                            WHERE FS."ElectionRoundId" = electionRoundId
                              AND FS."PollingStationId" = pollingStationId
                              AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        ) AS "NumberOfQuestionsAnswered",
                        (
                            SELECT COUNT(*)
                            FROM "QuickReports" QR
                            WHERE QR."ElectionRoundId" = electionRoundId
                              AND QR."PollingStationId" = pollingStationId
                              AND QR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        ) AS "NumberOfQuickReports",
                        (
                            SELECT COUNT(DISTINCT "MonitoringObserverId")
                            FROM (
                                SELECT FS."MonitoringObserverId"
                                FROM "FormSubmissions" FS
                                WHERE FS."ElectionRoundId" = electionRoundId
                                  AND FS."PollingStationId" = pollingStationId
                                  AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                                UNION
                                SELECT PSI."MonitoringObserverId"
                                FROM "PollingStationInformation" PSI
                                WHERE PSI."ElectionRoundId" = electionRoundId
                                  AND PSI."PollingStationId" = pollingStationId
                                  AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                                UNION
                                SELECT QR."MonitoringObserverId"
                                FROM "QuickReports" QR
                                WHERE QR."ElectionRoundId" = electionRoundId
                                  AND QR."PollingStationId" = pollingStationId
                                  AND QR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                            ) T
                        ) AS "NumberOfObservers",
                        EXISTS (
                            SELECT 1
                            FROM "PollingStationInformation" PSI
                            WHERE PSI."ElectionRoundId" = electionRoundId
                              AND PSI."PollingStationId" = pollingStationId
                              AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        ) AS "HasPollingStationInformation";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Overview stats (answers, flagged answers, observers, minutes, submissions, reports) for one polling station; platform admins see all observers of the election round, others the observers available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetPollingStationStatsOverview"(electionRoundId UUID, ngoId UUID, pollingStationId UUID, dataSource TEXT, isPlatformAdmin BOOLEAN)
                RETURNS TABLE (
                    "TotalAnswers" BIGINT,
                    "FlaggedAnswers" BIGINT,
                    "ObserversVisited" BIGINT,
                    "MinutesMonitoring" DOUBLE PRECISION,
                    "FormSubmissions" BIGINT,
                    "QuickReports" BIGINT,
                    "IncidentReports" BIGINT
                )
                AS $$
                    WITH "AvailableObservers" AS (
                        SELECT MO."Id" AS "MonitoringObserverId"
                        FROM "MonitoringObservers" MO
                        WHERE isPlatformAdmin
                          AND MO."ElectionRoundId" = electionRoundId
                        UNION ALL
                        SELECT AMO."MonitoringObserverId"
                        FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                        WHERE NOT isPlatformAdmin
                    )
                    SELECT
                        (
                            SELECT COALESCE((
                                SELECT SUM(FS."NumberOfQuestionsAnswered")
                                FROM "FormSubmissions" FS
                                WHERE FS."ElectionRoundId" = electionRoundId
                                  AND FS."PollingStationId" = pollingStationId
                                  AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                            ), 0)
                            + COALESCE((
                                SELECT SUM(PSI."NumberOfQuestionsAnswered")
                                FROM "PollingStationInformation" PSI
                                WHERE PSI."ElectionRoundId" = electionRoundId
                                  AND PSI."PollingStationId" = pollingStationId
                                  AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                            ), 0)
                        ) AS "TotalAnswers",
                        (
                            SELECT COALESCE((
                                SELECT SUM(FS."NumberOfFlaggedAnswers")
                                FROM "FormSubmissions" FS
                                WHERE FS."ElectionRoundId" = electionRoundId
                                  AND FS."PollingStationId" = pollingStationId
                                  AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                            ), 0)
                            + COALESCE((
                                SELECT SUM(PSI."NumberOfFlaggedAnswers")
                                FROM "PollingStationInformation" PSI
                                WHERE PSI."ElectionRoundId" = electionRoundId
                                  AND PSI."PollingStationId" = pollingStationId
                                  AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                            ), 0)
                        ) AS "FlaggedAnswers",
                        (
                            SELECT COUNT(DISTINCT "MonitoringObserverId")
                            FROM (
                                SELECT FS."MonitoringObserverId"
                                FROM "FormSubmissions" FS
                                WHERE FS."ElectionRoundId" = electionRoundId
                                  AND FS."PollingStationId" = pollingStationId
                                  AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                                UNION
                                SELECT PSI."MonitoringObserverId"
                                FROM "PollingStationInformation" PSI
                                WHERE PSI."ElectionRoundId" = electionRoundId
                                  AND PSI."PollingStationId" = pollingStationId
                                  AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                                UNION
                                SELECT QR."MonitoringObserverId"
                                FROM "QuickReports" QR
                                WHERE QR."ElectionRoundId" = electionRoundId
                                  AND QR."PollingStationId" = pollingStationId
                                  AND QR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                                UNION
                                SELECT IR."MonitoringObserverId"
                                FROM "IncidentReports" IR
                                WHERE IR."ElectionRoundId" = electionRoundId
                                  AND IR."PollingStationId" = pollingStationId
                                  AND IR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                            ) T
                        ) AS "ObserversVisited",
                        (
                            SELECT COALESCE(SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks")), 0)
                            FROM "PollingStationInformation" PSI
                            WHERE PSI."ElectionRoundId" = electionRoundId
                              AND PSI."PollingStationId" = pollingStationId
                              AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        ) AS "MinutesMonitoring",
                        (
                            SELECT COUNT(*)
                            FROM "FormSubmissions" FS
                            WHERE FS."ElectionRoundId" = electionRoundId
                              AND FS."PollingStationId" = pollingStationId
                              AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        ) AS "FormSubmissions",
                        (
                            SELECT COUNT(*)
                            FROM "QuickReports" QR
                            WHERE QR."ElectionRoundId" = electionRoundId
                              AND QR."PollingStationId" = pollingStationId
                              AND QR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        ) AS "QuickReports",
                        (
                            SELECT COUNT(*)
                            FROM "IncidentReports" IR
                            WHERE IR."ElectionRoundId" = electionRoundId
                              AND IR."PollingStationId" = pollingStationId
                              AND IR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        ) AS "IncidentReports";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Workload stats (visited stations, submissions, answers, minutes, reports) for one monitoring observer, if available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetMonitoringObserverStatsWorkload"(electionRoundId UUID, ngoId UUID, monitoringObserverId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "VisitedPollingStations" BIGINT,
                    "FormSubmissions" BIGINT,
                    "FlaggedAnswers" BIGINT,
                    "TotalAnswers" BIGINT,
                    "MinutesMonitoring" DOUBLE PRECISION,
                    "QuickReports" BIGINT,
                    "IncidentReports" BIGINT
                )
                AS $$
                    WITH "AvailableObservers" AS (
                        SELECT "MonitoringObserverId"
                        FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource)
                        WHERE "MonitoringObserverId" = monitoringObserverId
                    ),
                    "VisitedStations" AS (
                        SELECT FS."PollingStationId"
                        FROM "FormSubmissions" FS
                        WHERE FS."ElectionRoundId" = electionRoundId
                          AND FS."MonitoringObserverId" = monitoringObserverId
                          AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        UNION
                        SELECT PSI."PollingStationId"
                        FROM "PollingStationInformation" PSI
                        WHERE PSI."ElectionRoundId" = electionRoundId
                          AND PSI."MonitoringObserverId" = monitoringObserverId
                          AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        UNION
                        SELECT QR."PollingStationId"
                        FROM "QuickReports" QR
                        WHERE QR."ElectionRoundId" = electionRoundId
                          AND QR."MonitoringObserverId" = monitoringObserverId
                          AND QR."PollingStationId" IS NOT NULL
                          AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        UNION
                        SELECT IR."PollingStationId"
                        FROM "IncidentReports" IR
                        WHERE IR."ElectionRoundId" = electionRoundId
                          AND IR."MonitoringObserverId" = monitoringObserverId
                          AND IR."PollingStationId" IS NOT NULL
                          AND EXISTS (SELECT 1 FROM "AvailableObservers")
                    )
                    SELECT
                        (SELECT COUNT(*) FROM "VisitedStations") AS "VisitedPollingStations",
                        (
                            SELECT COUNT(*)
                            FROM "FormSubmissions" FS
                            WHERE FS."ElectionRoundId" = electionRoundId
                              AND FS."MonitoringObserverId" = monitoringObserverId
                              AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        ) AS "FormSubmissions",
                        (
                            SELECT COALESCE((
                                SELECT SUM(FS."NumberOfFlaggedAnswers")
                                FROM "FormSubmissions" FS
                                WHERE FS."ElectionRoundId" = electionRoundId
                                  AND FS."MonitoringObserverId" = monitoringObserverId
                            ), 0)
                            + COALESCE((
                                SELECT SUM(PSI."NumberOfFlaggedAnswers")
                                FROM "PollingStationInformation" PSI
                                WHERE PSI."ElectionRoundId" = electionRoundId
                                  AND PSI."MonitoringObserverId" = monitoringObserverId
                            ), 0)
                        ) AS "FlaggedAnswers",
                        (
                            SELECT COALESCE((
                                SELECT SUM(FS."NumberOfQuestionsAnswered")
                                FROM "FormSubmissions" FS
                                WHERE FS."ElectionRoundId" = electionRoundId
                                  AND FS."MonitoringObserverId" = monitoringObserverId
                            ), 0)
                            + COALESCE((
                                SELECT SUM(PSI."NumberOfQuestionsAnswered")
                                FROM "PollingStationInformation" PSI
                                WHERE PSI."ElectionRoundId" = electionRoundId
                                  AND PSI."MonitoringObserverId" = monitoringObserverId
                            ), 0)
                        ) AS "TotalAnswers",
                        (
                            SELECT COALESCE(SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks")), 0)
                            FROM "PollingStationInformation" PSI
                            WHERE PSI."ElectionRoundId" = electionRoundId
                              AND PSI."MonitoringObserverId" = monitoringObserverId
                              AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        ) AS "MinutesMonitoring",
                        (
                            SELECT COUNT(*)
                            FROM "QuickReports" QR
                            WHERE QR."ElectionRoundId" = electionRoundId
                              AND QR."MonitoringObserverId" = monitoringObserverId
                              AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        ) AS "QuickReports",
                        (
                            SELECT COUNT(*)
                            FROM "IncidentReports" IR
                            WHERE IR."ElectionRoundId" = electionRoundId
                              AND IR."MonitoringObserverId" = monitoringObserverId
                              AND EXISTS (SELECT 1 FROM "AvailableObservers")
                        ) AS "IncidentReports";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Totals for the observer's own activity in an election round (forms, answers, quick reports, notes, attachments, visited stations)
                CREATE OR REPLACE FUNCTION "GetObserverStatisticsTotals"(electionRoundId UUID, observerId UUID)
                RETURNS TABLE (
                    "NumberOfFormsSubmitted" NUMERIC,
                    "NumberOfQuestionsAnswered" NUMERIC,
                    "NumberOfQuickReports" BIGINT,
                    "NumberOfNotes" BIGINT,
                    "NumberOfAttachments" NUMERIC,
                    "NumberOfPollingStationsVisited" BIGINT
                )
                AS $$
                    WITH
                        "SubmissionsStats" AS (
                            SELECT
                                SUM("NumberOfFormsSubmitted") AS "NumberOfFormsSubmitted",
                                SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                            FROM
                                (
                                    SELECT
                                        COUNT(*) AS "NumberOfFormsSubmitted",
                                        SUM(PSI."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                                    FROM
                                        "PollingStationInformation" PSI
                                            INNER JOIN "MonitoringObservers" MO ON PSI."MonitoringObserverId" = MO."Id"
                                    WHERE
                                        PSI."ElectionRoundId" = electionRoundId
                                      AND MO."ObserverId" = observerId
                                    UNION ALL
                                    SELECT
                                        COUNT(*) AS "NumberOfFormsSubmitted",
                                        SUM(FS."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                                    FROM
                                        "FormSubmissions" FS
                                            INNER JOIN "MonitoringObservers" MO ON FS."MonitoringObserverId" = MO."Id"
                                    WHERE
                                        FS."ElectionRoundId" = electionRoundId
                                      AND MO."ObserverId" = observerId
                                ) AS "FormsSubmittedData"
                        ),
                        "QuickReportStats" AS (
                            SELECT
                                COUNT(*) AS "NumberOfQuickReports"
                            FROM
                                "QuickReports" QR
                                    INNER JOIN "MonitoringObservers" MO ON QR."MonitoringObserverId" = MO."Id"
                            WHERE
                                QR."ElectionRoundId" = electionRoundId
                              AND MO."ObserverId" = observerId
                        ),
                        "NoteStats" AS (
                            SELECT
                                COUNT(*) AS "NumberOfNotes"
                            FROM
                                "Notes" N
                            INNER JOIN "MonitoringObservers" MO ON N."MonitoringObserverId" = MO."Id"
                            WHERE
                                MO."ElectionRoundId" = electionRoundId
                              AND MO."ObserverId" = observerId
                        ),
                        "AttachmentsStats" AS (
                            SELECT
                                SUM("NumberOfAttachments") AS "NumberOfAttachments"
                            FROM
                                (
                                    SELECT
                                        COUNT(*) AS "NumberOfAttachments"
                                    FROM
                                        "Attachments" A
                                    INNER JOIN "MonitoringObservers" MO ON A."MonitoringObserverId" = MO."Id"
                                    WHERE
                                      MO."ElectionRoundId" = electionRoundId
                                      AND MO."ObserverId" = observerId
                                      AND A."IsDeleted" = FALSE
                                    UNION ALL
                                    SELECT
                                        COUNT(*) AS "NumberOfAttachments"
                                    FROM
                                        "QuickReportAttachments" QRA
                                            INNER JOIN "MonitoringObservers" MO ON QRA."MonitoringObserverId" = MO."Id"
                                    WHERE
                                        QRA."ElectionRoundId" = electionRoundId
                                      AND MO."ObserverId" = observerId
                                      AND QRA."IsDeleted" = FALSE
                                ) AS "AttachmentsData"
                        ),
                        "PollingStationsStats" AS (
                            SELECT
                                COUNT(DISTINCT "PollingStationId") AS "NumberOfPollingStationsVisited"
                            FROM
                                (
                                    SELECT
                                        PSI."PollingStationId"
                                    FROM
                                        "PollingStationInformation" PSI
                                            INNER JOIN "MonitoringObservers" MO ON PSI."MonitoringObserverId" = MO."Id"
                                    WHERE
                                        PSI."ElectionRoundId" = electionRoundId
                                      AND MO."ObserverId" = observerId
                                    UNION ALL
                                    SELECT
                                        FS."PollingStationId"
                                    FROM
                                        "FormSubmissions" FS
                                            INNER JOIN "MonitoringObservers" MO ON FS."MonitoringObserverId" = MO."Id"
                                    WHERE
                                        FS."ElectionRoundId" = electionRoundId
                                      AND MO."ObserverId" = observerId
                                    UNION ALL
                                    SELECT
                                        QR."PollingStationId"
                                    FROM
                                        "QuickReports" QR
                                            INNER JOIN "MonitoringObservers" MO ON QR."MonitoringObserverId" = MO."Id"
                                    WHERE
                                        QR."ElectionRoundId" = electionRoundId
                                      AND MO."ObserverId" = observerId
                                      AND QR."PollingStationId" IS NOT NULL
                                ) AS "PollingStationsVisitData"
                        )
                    SELECT
                        FS."NumberOfFormsSubmitted",
                        FS."NumberOfQuestionsAnswered",
                        QR."NumberOfQuickReports",
                        N."NumberOfNotes",
                        A."NumberOfAttachments",
                        PS."NumberOfPollingStationsVisited"
                    FROM
                        "SubmissionsStats" FS,
                        "QuickReportStats" QR,
                        "NoteStats" N,
                        "AttachmentsStats" A,
                        "PollingStationsStats" PS;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Per location level/path (level 0 = total) visited polling station stats for one monitoring observer, if available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetMonitoringObserverPollingStationLevelStats"(electionRoundId UUID, ngoId UUID, monitoringObserverId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "Path" TEXT,
                    "Level" INTEGER,
                    "NumberOfVisitedPollingStations" BIGINT,
                    "NumberOfPollingStations" BIGINT,
                    "NumberOfIncidentReports" NUMERIC,
                    "NumberOfQuickReports" NUMERIC,
                    "NumberOfFormSubmissions" NUMERIC,
                    "MinutesMonitoring" DOUBLE PRECISION,
                    "NumberOfFlaggedAnswers" NUMERIC,
                    "NumberOfQuestionsAnswered" NUMERIC,
                    "ActiveObservers" INTEGER,
                    "CoveragePercentage" NUMERIC
                )
                AS $$
                    WITH
                        "AvailableObservers" AS (
                            SELECT "MonitoringObserverId"
                            FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource)
                            WHERE "MonitoringObserverId" = monitoringObserverId
                        ),
                        "PollingStationsStatsRaw" AS (
                            SELECT
                                FS."PollingStationId",
                                0 AS "NumberOfIncidentReports",
                                0 AS "NumberOfQuickReports",
                                COUNT(1) AS "NumberOfFormSubmissions",
                                0::float AS "MinutesMonitoring",
                                SUM(FS."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                                SUM(FS."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                            FROM "FormSubmissions" FS
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                            GROUP BY FS."PollingStationId"
                            UNION ALL
                            SELECT
                                PSI."PollingStationId",
                                0 AS "NumberOfIncidentReports",
                                0 AS "NumberOfQuickReports",
                                COUNT(1) AS "NumberOfFormSubmissions",
                                SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks")) AS "MinutesMonitoring",
                                SUM(PSI."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                                SUM(PSI."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                            FROM "PollingStationInformation" PSI
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                            GROUP BY PSI."PollingStationId"
                            UNION ALL
                            SELECT
                                QR."PollingStationId",
                                0 AS "NumberOfIncidentReports",
                                COUNT(1) AS "NumberOfQuickReports",
                                0 AS "NumberOfFormSubmissions",
                                0::float AS "MinutesMonitoring",
                                0 AS "NumberOfFlaggedAnswers",
                                0 AS "NumberOfQuestionsAnswered"
                            FROM "QuickReports" QR
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                            WHERE QR."PollingStationId" IS NOT NULL
                            GROUP BY QR."PollingStationId"
                            UNION ALL
                            SELECT
                                IR."PollingStationId",
                                COUNT(1) AS "NumberOfIncidentReports",
                                0 AS "NumberOfQuickReports",
                                0 AS "NumberOfFormSubmissions",
                                0::float AS "MinutesMonitoring",
                                0 AS "NumberOfFlaggedAnswers",
                                0 AS "NumberOfQuestionsAnswered"
                            FROM "IncidentReports" IR
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                            WHERE IR."PollingStationId" IS NOT NULL
                            GROUP BY IR."PollingStationId"
                        ),
                        "PollingStationsStats" AS (
                            SELECT
                                "PollingStationId",
                                SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                                SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                                SUM("NumberOfFormSubmissions") AS "NumberOfFormSubmissions",
                                SUM("MinutesMonitoring") AS "MinutesMonitoring",
                                SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                                SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                            FROM "PollingStationsStatsRaw"
                            GROUP BY "PollingStationId"
                        ),
                        "ElectionPollingStations" AS (
                            SELECT
                                PS."Id",
                                NULLIF(PS."Level1", '') AS "Level1",
                                NULLIF(PS."Level2", '') AS "Level2",
                                NULLIF(PS."Level3", '') AS "Level3",
                                NULLIF(PS."Level4", '') AS "Level4",
                                NULLIF(PS."Level5", '') AS "Level5"
                            FROM "PollingStations" PS
                            WHERE PS."ElectionRoundId" = electionRoundId
                              AND PS."Level1" != ''
                        ),
                        "PollingStationsPerLevel" AS (
                            SELECT
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN 0
                                    WHEN GROUPING(PS."Level2") = 1 THEN 1
                                    WHEN GROUPING(PS."Level3") = 1 THEN 2
                                    WHEN GROUPING(PS."Level4") = 1 THEN 3
                                    WHEN GROUPING(PS."Level5") = 1 THEN 4
                                    ELSE 5
                                END AS "Level",
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN '/'
                                    ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                                END AS "Path",
                                COUNT(PS."Id") AS "NumberOfPollingStations"
                            FROM "ElectionPollingStations" PS
                            GROUP BY GROUPING SETS (
                                (),
                                (PS."Level1"),
                                (PS."Level1", PS."Level2"),
                                (PS."Level1", PS."Level2", PS."Level3"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                            )
                            HAVING
                                GROUPING(PS."Level1") = 1
                                OR (
                                    GROUPING(PS."Level2") = 1
                                    AND PS."Level1" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level3") = 1
                                    AND GROUPING(PS."Level2") = 0
                                    AND PS."Level2" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level4") = 1
                                    AND GROUPING(PS."Level3") = 0
                                    AND PS."Level3" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 1
                                    AND GROUPING(PS."Level4") = 0
                                    AND PS."Level4" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 0
                                    AND PS."Level5" IS NOT NULL
                                )
                        ),
                        "PollingStationLevelsStats" AS (
                            SELECT
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN 0
                                    WHEN GROUPING(PS."Level2") = 1 THEN 1
                                    WHEN GROUPING(PS."Level3") = 1 THEN 2
                                    WHEN GROUPING(PS."Level4") = 1 THEN 3
                                    WHEN GROUPING(PS."Level5") = 1 THEN 4
                                    ELSE 5
                                END AS "Level",
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN '/'
                                    ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                                END AS "Path",
                                COUNT(PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                                COALESCE(SUM(PSV."NumberOfIncidentReports"), 0) AS "NumberOfIncidentReports",
                                COALESCE(SUM(PSV."NumberOfQuickReports"), 0) AS "NumberOfQuickReports",
                                COALESCE(SUM(PSV."NumberOfFormSubmissions"), 0) AS "NumberOfFormSubmissions",
                                COALESCE(SUM(PSV."MinutesMonitoring"), 0) AS "MinutesMonitoring",
                                COALESCE(SUM(PSV."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                                COALESCE(SUM(PSV."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered"
                            FROM "PollingStationsStats" PSV
                                INNER JOIN "ElectionPollingStations" PS ON PS."Id" = PSV."PollingStationId"
                            GROUP BY GROUPING SETS (
                                (),
                                (PS."Level1"),
                                (PS."Level1", PS."Level2"),
                                (PS."Level1", PS."Level2", PS."Level3"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                            )
                            HAVING
                                GROUPING(PS."Level1") = 1
                                OR (
                                    GROUPING(PS."Level2") = 1
                                    AND PS."Level1" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level3") = 1
                                    AND GROUPING(PS."Level2") = 0
                                    AND PS."Level2" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level4") = 1
                                    AND GROUPING(PS."Level3") = 0
                                    AND PS."Level3" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 1
                                    AND GROUPING(PS."Level4") = 0
                                    AND PS."Level4" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 0
                                    AND PS."Level5" IS NOT NULL
                                )
                        )
                    SELECT
                        S."Path",
                        S."Level",
                        S."NumberOfVisitedPollingStations",
                        PS."NumberOfPollingStations",
                        S."NumberOfIncidentReports",
                        S."NumberOfQuickReports",
                        S."NumberOfFormSubmissions",
                        S."MinutesMonitoring",
                        S."NumberOfFlaggedAnswers",
                        S."NumberOfQuestionsAnswered",
                        CASE WHEN S."NumberOfVisitedPollingStations" > 0 THEN 1 ELSE 0 END AS "ActiveObservers",
                        (
                            S."NumberOfVisitedPollingStations" * 100.0 / NULLIF(PS."NumberOfPollingStations", 0)
                        ) AS "CoveragePercentage"
                    FROM "PollingStationLevelsStats" S
                        INNER JOIN "PollingStationsPerLevel" PS ON S."Level" = PS."Level" AND S."Path" = PS."Path";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Notes, attachments, quick reports and incident reports counts for one monitoring observer, if available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetMonitoringObserverStatisticsExtras"(electionRoundId UUID, ngoId UUID, monitoringObserverId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "NumberOfNotes" BIGINT,
                    "NumberOfAttachments" NUMERIC,
                    "NumberOfQuickReports" BIGINT,
                    "NumberOfIncidentReports" BIGINT
                )
                AS $$
                    WITH "AvailableObservers" AS (
                        SELECT "MonitoringObserverId"
                        FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource)
                        WHERE "MonitoringObserverId" = monitoringObserverId
                    )
                    SELECT
                        (SELECT COUNT(*) FROM "Notes" N WHERE N."MonitoringObserverId" = monitoringObserverId AND EXISTS (SELECT 1 FROM "AvailableObservers")) AS "NumberOfNotes",
                        (
                            SELECT COALESCE(SUM(c), 0)
                            FROM (
                                SELECT COUNT(*) AS c
                                FROM "Attachments" A
                                WHERE A."MonitoringObserverId" = monitoringObserverId
                                  AND A."IsDeleted" = FALSE
                                  AND EXISTS (SELECT 1 FROM "AvailableObservers")
                                UNION ALL
                                SELECT COUNT(*) AS c
                                FROM "QuickReportAttachments" QRA
                                WHERE QRA."ElectionRoundId" = electionRoundId
                                  AND QRA."MonitoringObserverId" = monitoringObserverId
                                  AND QRA."IsDeleted" = FALSE
                                  AND EXISTS (SELECT 1 FROM "AvailableObservers")
                            ) attachments
                        ) AS "NumberOfAttachments",
                        (SELECT COUNT(*) FROM "QuickReports" QR WHERE QR."ElectionRoundId" = electionRoundId AND QR."MonitoringObserverId" = monitoringObserverId AND EXISTS (SELECT 1 FROM "AvailableObservers")) AS "NumberOfQuickReports",
                        (SELECT COUNT(*) FROM "IncidentReports" IR WHERE IR."ElectionRoundId" = electionRoundId AND IR."MonitoringObserverId" = monitoringObserverId AND EXISTS (SELECT 1 FROM "AvailableObservers")) AS "NumberOfIncidentReports";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Hourly histogram of answers and flagged answers for one monitoring observer, if available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetMonitoringObserverStatisticsAnswersHistogram"(electionRoundId UUID, ngoId UUID, monitoringObserverId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "Bucket" TIMESTAMPTZ,
                    "NumberOfAnswers" BIGINT,
                    "NumberOfFlaggedAnswers" NUMERIC
                )
                AS $$
                    WITH "AvailableObservers" AS (
                        SELECT "MonitoringObserverId"
                        FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource)
                        WHERE "MonitoringObserverId" = monitoringObserverId
                    ),
                    "Submissions" AS (
                        SELECT FS."Answers", F."Questions", FS."LastUpdatedAt", FS."CreatedAt"
                        FROM "FormSubmissions" FS
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                            INNER JOIN "Forms" F ON F."Id" = FS."FormId"

                        UNION ALL

                        SELECT PSI."Answers", PSIF."Questions", PSI."LastUpdatedAt", NULL::timestamptz AS "CreatedAt"
                        FROM "PollingStationInformation" PSI
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                            INNER JOIN "PollingStationInformationForms" PSIF ON PSIF."Id" = PSI."PollingStationInformationFormId"
                    ),
                    -- answers to the form's questions, timed by the answer's own timestamps when the client sent them
                    "SubmissionAnswers" AS (
                        SELECT
                            COALESCE(
                                (A ->> 'LastUpdatedAt')::timestamptz,
                                (A ->> 'CreatedAt')::timestamptz,
                                S."LastUpdatedAt",
                                S."CreatedAt"
                            ) AS answered_at,
                            -- flagged options selected in a single or multi select answer
                            CASE
                                WHEN A ->> '$answerType' = 'singleSelectAnswer' AND Q ->> '$questionType' = 'singleSelectQuestion'
                                    THEN (SELECT COUNT(*)
                                          FROM JSONB_ARRAY_ELEMENTS(Q -> 'Options') O
                                          WHERE (O ->> 'IsFlagged')::boolean
                                            AND O ->> 'Id' = A -> 'Selection' ->> 'OptionId')
                                WHEN A ->> '$answerType' = 'multiSelectAnswer' AND Q ->> '$questionType' = 'multiSelectQuestion'
                                    THEN (SELECT COUNT(*)
                                          FROM JSONB_ARRAY_ELEMENTS(Q -> 'Options') O
                                          WHERE (O ->> 'IsFlagged')::boolean
                                            AND EXISTS (SELECT 1
                                                        FROM JSONB_ARRAY_ELEMENTS(A -> 'Selection') SEL
                                                        WHERE SEL ->> 'OptionId' = O ->> 'Id'))
                                ELSE 0
                            END AS number_of_flagged_answers
                        FROM "Submissions" S
                            CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Answers") A
                            INNER JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Questions") Q ON Q ->> 'Id' = A ->> 'QuestionId'
                    )
                    SELECT
                        DATE_TRUNC(
                                'hour',
                                TIMEZONE('utc', answered_at)
                        )::timestamptz AS "Bucket",
                        COUNT(*) AS "NumberOfAnswers",
                        SUM(number_of_flagged_answers) AS "NumberOfFlaggedAnswers"
                    FROM "SubmissionAnswers"
                    GROUP BY 1
                    ORDER BY 1;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Active / pending / suspended observer counts available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetNgoAdminStatisticsObservers"(electionRoundId UUID, ngoId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "ActiveObservers" BIGINT,
                    "PendingObservers" BIGINT,
                    "SuspendedObservers" BIGINT
                )
                AS $$
                    SELECT
                                COUNT("MonitoringObserverId") FILTER (
                            WHERE
                            "Status" = 'Active'
                                AND "AccountStatus" = 'Active'
                            ) "ActiveObservers",
                                COUNT("MonitoringObserverId") FILTER (
                                    WHERE
                                    "Status" = 'Pending'
                                        OR "AccountStatus" = 'Pending'
                                    ) "PendingObservers",
                                COUNT("MonitoringObserverId") FILTER (
                                    WHERE
                                    "Status" = 'Suspended'
                                        OR "AccountStatus" = 'Suspended'
                                    ) "SuspendedObservers"
                    FROM
                        "GetAvailableMonitoringObservers" (electionRoundId, ngoId, dataSource);
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Per location level/path (level 0 = total) visited polling station stats and active observers for the observers available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetNgoAdminStatisticsLevels"(electionRoundId UUID, ngoId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "Path" TEXT,
                    "Level" INTEGER,
                    "NumberOfVisitedPollingStations" BIGINT,
                    "NumberOfPollingStations" BIGINT,
                    "NumberOfIncidentReports" NUMERIC,
                    "NumberOfQuickReports" NUMERIC,
                    "NumberOfFormSubmissions" NUMERIC,
                    "MinutesMonitoring" DOUBLE PRECISION,
                    "NumberOfFlaggedAnswers" NUMERIC,
                    "NumberOfQuestionsAnswered" NUMERIC,
                    "ActiveObservers" BIGINT,
                    "CoveragePercentage" NUMERIC
                )
                AS $$
                    WITH
                        "AvailableObservers" AS (
                            SELECT "MonitoringObserverId"
                            FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource)
                        ),
                        "ActiveObservers" AS (
                            SELECT
                                FS."PollingStationId",
                                FS."MonitoringObserverId"
                            FROM "FormSubmissions" FS
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                            UNION
                            SELECT
                                PSI."PollingStationId",
                                PSI."MonitoringObserverId"
                            FROM "PollingStationInformation" PSI
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                            UNION
                            SELECT
                                QR."PollingStationId",
                                QR."MonitoringObserverId"
                            FROM "QuickReports" QR
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                            WHERE QR."PollingStationId" IS NOT NULL
                            UNION
                            SELECT
                                IR."PollingStationId",
                                IR."MonitoringObserverId"
                            FROM "IncidentReports" IR
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                            WHERE IR."PollingStationId" IS NOT NULL
                        ),
                        "PollingStationsStatsRaw" AS (
                            SELECT
                                FS."PollingStationId",
                                0 AS "NumberOfIncidentReports",
                                0 AS "NumberOfQuickReports",
                                COUNT(1) AS "NumberOfFormSubmissions",
                                0::float AS "MinutesMonitoring",
                                SUM(FS."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                                SUM(FS."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                            FROM "FormSubmissions" FS
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                            GROUP BY FS."PollingStationId"
                            UNION ALL
                            SELECT
                                PSI."PollingStationId",
                                0 AS "NumberOfIncidentReports",
                                0 AS "NumberOfQuickReports",
                                COUNT(1) AS "NumberOfFormSubmissions",
                                SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks")) AS "MinutesMonitoring",
                                SUM(PSI."NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                                SUM(PSI."NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                            FROM "PollingStationInformation" PSI
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                            GROUP BY PSI."PollingStationId"
                            UNION ALL
                            SELECT
                                QR."PollingStationId",
                                0 AS "NumberOfIncidentReports",
                                COUNT(1) AS "NumberOfQuickReports",
                                0 AS "NumberOfFormSubmissions",
                                0::float AS "MinutesMonitoring",
                                0 AS "NumberOfFlaggedAnswers",
                                0 AS "NumberOfQuestionsAnswered"
                            FROM "QuickReports" QR
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                            WHERE QR."PollingStationId" IS NOT NULL
                            GROUP BY QR."PollingStationId"
                            UNION ALL
                            SELECT
                                IR."PollingStationId",
                                COUNT(1) AS "NumberOfIncidentReports",
                                0 AS "NumberOfQuickReports",
                                0 AS "NumberOfFormSubmissions",
                                0::float AS "MinutesMonitoring",
                                0 AS "NumberOfFlaggedAnswers",
                                0 AS "NumberOfQuestionsAnswered"
                            FROM "IncidentReports" IR
                                INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                            WHERE IR."PollingStationId" IS NOT NULL
                            GROUP BY IR."PollingStationId"
                        ),
                        "PollingStationsStats" AS (
                            SELECT
                                "PollingStationId",
                                SUM("NumberOfIncidentReports") AS "NumberOfIncidentReports",
                                SUM("NumberOfQuickReports") AS "NumberOfQuickReports",
                                SUM("NumberOfFormSubmissions") AS "NumberOfFormSubmissions",
                                SUM("MinutesMonitoring") AS "MinutesMonitoring",
                                SUM("NumberOfFlaggedAnswers") AS "NumberOfFlaggedAnswers",
                                SUM("NumberOfQuestionsAnswered") AS "NumberOfQuestionsAnswered"
                            FROM "PollingStationsStatsRaw"
                            GROUP BY "PollingStationId"
                        ),
                        "ElectionPollingStations" AS (
                            SELECT
                                PS."Id",
                                NULLIF(PS."Level1", '') AS "Level1",
                                NULLIF(PS."Level2", '') AS "Level2",
                                NULLIF(PS."Level3", '') AS "Level3",
                                NULLIF(PS."Level4", '') AS "Level4",
                                NULLIF(PS."Level5", '') AS "Level5"
                            FROM "PollingStations" PS
                            WHERE PS."ElectionRoundId" = electionRoundId
                              AND PS."Level1" != ''
                        ),
                        "PollingStationsPerLevel" AS (
                            SELECT
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN 0
                                    WHEN GROUPING(PS."Level2") = 1 THEN 1
                                    WHEN GROUPING(PS."Level3") = 1 THEN 2
                                    WHEN GROUPING(PS."Level4") = 1 THEN 3
                                    WHEN GROUPING(PS."Level5") = 1 THEN 4
                                    ELSE 5
                                END AS "Level",
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN '/'
                                    ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                                END AS "Path",
                                COUNT(PS."Id") AS "NumberOfPollingStations"
                            FROM "ElectionPollingStations" PS
                            GROUP BY GROUPING SETS (
                                (),
                                (PS."Level1"),
                                (PS."Level1", PS."Level2"),
                                (PS."Level1", PS."Level2", PS."Level3"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                            )
                            HAVING
                                GROUPING(PS."Level1") = 1
                                OR (
                                    GROUPING(PS."Level2") = 1
                                    AND PS."Level1" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level3") = 1
                                    AND GROUPING(PS."Level2") = 0
                                    AND PS."Level2" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level4") = 1
                                    AND GROUPING(PS."Level3") = 0
                                    AND PS."Level3" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 1
                                    AND GROUPING(PS."Level4") = 0
                                    AND PS."Level4" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 0
                                    AND PS."Level5" IS NOT NULL
                                )
                        ),
                        "PollingStationLevelsStats" AS (
                            SELECT
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN 0
                                    WHEN GROUPING(PS."Level2") = 1 THEN 1
                                    WHEN GROUPING(PS."Level3") = 1 THEN 2
                                    WHEN GROUPING(PS."Level4") = 1 THEN 3
                                    WHEN GROUPING(PS."Level5") = 1 THEN 4
                                    ELSE 5
                                END AS "Level",
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN '/'
                                    ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                                END AS "Path",
                                COUNT(PSV."PollingStationId") AS "NumberOfVisitedPollingStations",
                                COALESCE(SUM(PSV."NumberOfIncidentReports"), 0) AS "NumberOfIncidentReports",
                                COALESCE(SUM(PSV."NumberOfQuickReports"), 0) AS "NumberOfQuickReports",
                                COALESCE(SUM(PSV."NumberOfFormSubmissions"), 0) AS "NumberOfFormSubmissions",
                                COALESCE(SUM(PSV."MinutesMonitoring"), 0) AS "MinutesMonitoring",
                                COALESCE(SUM(PSV."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                                COALESCE(SUM(PSV."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered"
                            FROM "PollingStationsStats" PSV
                                INNER JOIN "ElectionPollingStations" PS ON PS."Id" = PSV."PollingStationId"
                            GROUP BY GROUPING SETS (
                                (),
                                (PS."Level1"),
                                (PS."Level1", PS."Level2"),
                                (PS."Level1", PS."Level2", PS."Level3"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                            )
                            HAVING
                                GROUPING(PS."Level1") = 1
                                OR (
                                    GROUPING(PS."Level2") = 1
                                    AND PS."Level1" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level3") = 1
                                    AND GROUPING(PS."Level2") = 0
                                    AND PS."Level2" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level4") = 1
                                    AND GROUPING(PS."Level3") = 0
                                    AND PS."Level3" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 1
                                    AND GROUPING(PS."Level4") = 0
                                    AND PS."Level4" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 0
                                    AND PS."Level5" IS NOT NULL
                                )
                        ),
                        "ActiveObserversPerLevel" AS (
                            SELECT
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN 0
                                    WHEN GROUPING(PS."Level2") = 1 THEN 1
                                    WHEN GROUPING(PS."Level3") = 1 THEN 2
                                    WHEN GROUPING(PS."Level4") = 1 THEN 3
                                    WHEN GROUPING(PS."Level5") = 1 THEN 4
                                    ELSE 5
                                END AS "Level",
                                CASE
                                    WHEN GROUPING(PS."Level1") = 1 THEN '/'
                                    ELSE CONCAT_WS(' / ', PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                                END AS "Path",
                                COUNT(DISTINCT AO."MonitoringObserverId") AS "ActiveObservers"
                            FROM "ActiveObservers" AO
                                INNER JOIN "ElectionPollingStations" PS ON PS."Id" = AO."PollingStationId"
                            GROUP BY GROUPING SETS (
                                (),
                                (PS."Level1"),
                                (PS."Level1", PS."Level2"),
                                (PS."Level1", PS."Level2", PS."Level3"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4"),
                                (PS."Level1", PS."Level2", PS."Level3", PS."Level4", PS."Level5")
                            )
                            HAVING
                                GROUPING(PS."Level1") = 1
                                OR (
                                    GROUPING(PS."Level2") = 1
                                    AND PS."Level1" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level3") = 1
                                    AND GROUPING(PS."Level2") = 0
                                    AND PS."Level2" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level4") = 1
                                    AND GROUPING(PS."Level3") = 0
                                    AND PS."Level3" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 1
                                    AND GROUPING(PS."Level4") = 0
                                    AND PS."Level4" IS NOT NULL
                                )
                                OR (
                                    GROUPING(PS."Level5") = 0
                                    AND PS."Level5" IS NOT NULL
                                )
                        )
                    SELECT
                        S."Path",
                        S."Level",
                        S."NumberOfVisitedPollingStations",
                        PS."NumberOfPollingStations",
                        S."NumberOfIncidentReports",
                        S."NumberOfQuickReports",
                        S."NumberOfFormSubmissions",
                        S."MinutesMonitoring",
                        S."NumberOfFlaggedAnswers",
                        S."NumberOfQuestionsAnswered",
                        AOL."ActiveObservers",
                        (
                            S."NumberOfVisitedPollingStations" * 100.0 / NULLIF(PS."NumberOfPollingStations", 0)
                        ) AS "CoveragePercentage"
                    FROM "PollingStationLevelsStats" S
                        INNER JOIN "PollingStationsPerLevel" PS ON S."Level" = PS."Level" AND S."Path" = PS."Path"
                        INNER JOIN "ActiveObserversPerLevel" AOL ON S."Level" = AOL."Level" AND S."Path" = AOL."Path";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Hourly histogram of submitted forms (incl. PSI), answers and flagged answers for the observers available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetNgoAdminStatisticsFormsHistogram"(electionRoundId UUID, ngoId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "Bucket" TIMESTAMPTZ,
                    "FormsSubmitted" BIGINT,
                    "NumberOfQuestionsAnswered" BIGINT,
                    "NumberOfFlaggedAnswers" BIGINT
                )
                AS $$
                    WITH "AvailableObservers" AS (
                        SELECT "MonitoringObserverId"
                        FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource)
                    ),
                    submission_histogram AS (
                        SELECT
                            DATE_TRUNC(
                                    'hour',
                                    TIMEZONE('utc', FS."LastUpdatedAt")
                            )::timestamptz AS bucket,
                            1 AS forms_submitted,
                            FS."NumberOfQuestionsAnswered" AS number_of_questions_answered,
                            FS."NumberOfFlaggedAnswers" AS number_of_flagged_answers
                        FROM "FormSubmissions" FS
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"

                        UNION ALL

                        SELECT
                            DATE_TRUNC(
                                    'hour',
                                    TIMEZONE('utc', qr."LastUpdatedAt")
                            )::timestamptz AS bucket,
                            1 AS forms_submitted,
                            qr."NumberOfQuestionsAnswered" AS number_of_questions_answered,
                            qr."NumberOfFlaggedAnswers" AS number_of_flagged_answers
                        FROM "PollingStationInformation" qr
                            INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = qr."MonitoringObserverId"
                    )
                    SELECT
                        bucket AS "Bucket",
                        COUNT(*) AS "FormsSubmitted",
                        SUM(number_of_questions_answered) AS "NumberOfQuestionsAnswered",
                        SUM(number_of_flagged_answers) AS "NumberOfFlaggedAnswers"
                    FROM submission_histogram
                    GROUP BY bucket
                    ORDER BY bucket;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Hourly histogram of quick reports for the observers available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetNgoAdminStatisticsQuickReportsHistogram"(electionRoundId UUID, ngoId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "Bucket" TIMESTAMPTZ,
                    "Value" BIGINT
                )
                AS $$
                    WITH "AvailableObservers" AS (
                        SELECT "MonitoringObserverId"
                        FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource)
                    )
                    SELECT
                        DATE_TRUNC(
                                'hour',
                                TIMEZONE('utc', QR."LastUpdatedAt")
                        )::TIMESTAMPTZ "Bucket",
                        COUNT(1) "Value"
                    FROM "QuickReports" QR
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                    GROUP BY 1;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Hourly histogram of citizen reports of the election round, when the NGO is the citizen reporting NGO
                CREATE OR REPLACE FUNCTION "GetNgoAdminStatisticsCitizenReportsHistogram"(electionRoundId UUID, ngoId UUID)
                RETURNS TABLE (
                    "Bucket" TIMESTAMPTZ,
                    "Value" BIGINT
                )
                AS $$
                    SELECT
                        DATE_TRUNC(
                                'hour',
                                TIMEZONE(
                                        'utc',
                                        COALESCE(CR."LastModifiedOn", CR."CreatedOn")
                                )
                        )::TIMESTAMPTZ "Bucket",
                        COUNT(1) "Value"
                    FROM "CitizenReports" CR
                        INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                    WHERE CR."ElectionRoundId" = electionRoundId
                      AND MN."NgoId" = ngoId
                    GROUP BY 1;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Hourly histogram of incident reports for the observers available to the NGO / data source
                CREATE OR REPLACE FUNCTION "GetNgoAdminStatisticsIncidentReportsHistogram"(electionRoundId UUID, ngoId UUID, dataSource TEXT)
                RETURNS TABLE (
                    "Bucket" TIMESTAMPTZ,
                    "Value" BIGINT
                )
                AS $$
                    WITH "AvailableObservers" AS (
                        SELECT "MonitoringObserverId"
                        FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource)
                    )
                    SELECT
                        DATE_TRUNC(
                                'hour',
                                TIMEZONE('utc', IR."LastUpdatedAt")
                        )::TIMESTAMPTZ "Bucket",
                        COUNT(1) "Value"
                    FROM "IncidentReports" IR
                        INNER JOIN "AvailableObservers" MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                    GROUP BY 1;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of election rounds per country, ordered by country name
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsCountriesHistogram"()
                RETURNS TABLE (
                    "CountryId" UUID,
                    "CountryName" TEXT,
                    "Iso2" TEXT,
                    "NumberOfElections" BIGINT
                )
                AS $$
                    SELECT
                        C."Id" AS "CountryId",
                        C."Name"::TEXT AS "CountryName",
                        C."Iso2"::TEXT AS "Iso2",
                        COUNT(ER."Id") AS "NumberOfElections"
                    FROM "ElectionRounds" ER
                        INNER JOIN "Countries" C ON C."Id" = ER."CountryId"
                    GROUP BY C."Id", C."Name", C."Iso2"
                    ORDER BY C."Name";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide active / suspended observer accounts
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsObservers"()
                RETURNS TABLE (
                    "ActiveObservers" BIGINT,
                    "SuspendedObservers" BIGINT
                )
                AS $$
                    SELECT
                        COUNT(*) FILTER (WHERE U."Status" = 'Active') AS "ActiveObservers",
                        COUNT(*) FILTER (WHERE U."Status" = 'Deactivated') AS "SuspendedObservers"
                    FROM "Observers" O
                        INNER JOIN "AspNetUsers" U ON U."Id" = O."Id";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide activated / deactivated NGOs
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsNgos"()
                RETURNS TABLE (
                    "ActiveNgos" BIGINT,
                    "DeactivatedNgos" BIGINT
                )
                AS $$
                    SELECT
                        COUNT(*) FILTER (WHERE "Status" = 'Activated') AS "ActiveNgos",
                        COUNT(*) FILTER (WHERE "Status" = 'Deactivated') AS "DeactivatedNgos"
                    FROM "Ngos";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide number of election rounds
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsElectionRounds"()
                RETURNS BIGINT
                AS $$
                    SELECT COUNT(*) FROM "ElectionRounds";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide number of polling stations
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsPollingStations"()
                RETURNS BIGINT
                AS $$
                    SELECT COUNT(*) FROM "PollingStations";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide number of distinct polling stations with answered (non-draft) forms or PSI
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsVisitedPollingStations"()
                RETURNS BIGINT
                AS $$
                    SELECT COUNT(DISTINCT T."PollingStationId")
                    FROM (
                        SELECT FS."PollingStationId"
                        FROM "FormSubmissions" FS
                            INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                        WHERE F."Status" <> 'Drafted'
                          AND FS."NumberOfQuestionsAnswered" > 0
                        UNION
                        SELECT PSI."PollingStationId"
                        FROM "PollingStationInformation" PSI
                        WHERE PSI."NumberOfQuestionsAnswered" > 0
                    ) AS T;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide minutes monitoring (sum over PSI)
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsMinutesMonitoring"()
                RETURNS DOUBLE PRECISION
                AS $$
                    SELECT COALESCE(SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks")), 0)
                    FROM "PollingStationInformation";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide number of form submissions (non-draft forms, with answers) plus PSI with answers
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsFormSubmissions"()
                RETURNS BIGINT
                AS $$
                    SELECT
                        (
                            SELECT COUNT(1)
                            FROM "FormSubmissions" FS
                                INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                            WHERE FS."NumberOfQuestionsAnswered" > 0
                              AND F."Status" <> 'Drafted'
                        ) + (
                            SELECT COUNT(1)
                            FROM "PollingStationInformation"
                            WHERE "NumberOfQuestionsAnswered" > 0
                        );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide number of questions answered (non-draft form submissions plus PSI)
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsQuestionsAnswered"()
                RETURNS BIGINT
                AS $$
                    SELECT
                        (
                            SELECT COALESCE(SUM("NumberOfQuestionsAnswered"), 0)
                            FROM "FormSubmissions" FS
                                INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                            WHERE FS."NumberOfQuestionsAnswered" > 0
                              AND F."Status" <> 'Drafted'
                        ) + (
                            SELECT COALESCE(SUM("NumberOfQuestionsAnswered"), 0)
                            FROM "PollingStationInformation"
                        );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide number of flagged answers on non-draft form submissions
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsFlaggedAnswers"()
                RETURNS BIGINT
                AS $$
                    SELECT COALESCE(SUM(FS."NumberOfFlaggedAnswers"), 0)
                    FROM "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    WHERE FS."NumberOfQuestionsAnswered" > 0
                      AND F."Status" <> 'Drafted';
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide number of quick reports
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsQuickReports"()
                RETURNS BIGINT
                AS $$
                    SELECT COUNT(*) FROM "QuickReports";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide number of incident reports
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsIncidentReports"()
                RETURNS BIGINT
                AS $$
                    SELECT COUNT(*) FROM "IncidentReports";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Platform-wide number of citizen reports
                CREATE OR REPLACE FUNCTION "GetPlatformAdminStatisticsCitizenReports"()
                RETURNS BIGINT
                AS $$
                    SELECT COUNT(*) FROM "CitizenReports";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of active monitoring observers (active account) in the given election rounds
                CREATE OR REPLACE FUNCTION "GetPlatformStatisticsMonitoringObservers"(electionRoundIds UUID[])
                RETURNS BIGINT
                AS $$
                    SELECT
                        COUNT(*)
                    FROM
                        "MonitoringObservers" MO
                        LEFT JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                    WHERE
                        "ElectionRoundId" = ANY (electionRoundIds)
                        AND U."Status" = 'Active'
                        AND MO."Status" = 'Active';
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of active monitoring NGOs in the given election rounds
                CREATE OR REPLACE FUNCTION "GetPlatformStatisticsMonitoringNgos"(electionRoundIds UUID[])
                RETURNS BIGINT
                AS $$
                    SELECT
                        COUNT(*)
                    FROM
                        "MonitoringNgos"
                    WHERE
                        "ElectionRoundId" = ANY (electionRoundIds)
                        AND "Status" = 'Active';
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of polling stations in the given election rounds
                CREATE OR REPLACE FUNCTION "GetPlatformStatisticsPollingStations"(electionRoundIds UUID[])
                RETURNS BIGINT
                AS $$
                    SELECT
                        COUNT(*)
                    FROM
                        "PollingStations"
                    WHERE
                        "ElectionRoundId" = ANY (electionRoundIds);
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of visited polling stations and covered locations per level in the given election rounds
                CREATE OR REPLACE FUNCTION "GetPlatformStatisticsPollingStationVisits"(electionRoundIds UUID[])
                RETURNS TABLE (
                    "NumberOfVisitedPollingStations" BIGINT,
                    "NumberOfLevel1Covered" BIGINT,
                    "NumberOfLevel2Covered" BIGINT,
                    "NumberOfLevel3Covered" BIGINT,
                    "NumberOfLevel4Covered" BIGINT,
                    "NumberOfLevel5Covered" BIGINT
                )
                AS $$
                    SELECT
                        COUNT(DISTINCT T."PollingStationId") AS "NumberOfVisitedPollingStations",
                        COUNT(DISTINCT PS."Level1") AS "NumberOfLevel1Covered",
                        COUNT(DISTINCT CONCAT(PS."Level1", '-', PS."Level2")) AS "NumberOfLevel2Covered",
                        COUNT(
                            DISTINCT CONCAT(PS."Level1", '-', PS."Level2", '-', PS."Level3")
                        ) AS "NumberOfLevel3Covered",
                        COUNT(
                            DISTINCT CONCAT(
                                PS."Level1",
                                '-',
                                PS."Level2",
                                '-',
                                PS."Level3",
                                '-',
                                PS."Level4"
                            )
                        ) AS "NumberOfLevel4Covered",
                        COUNT(
                            DISTINCT CONCAT(
                                PS."Level1",
                                '-',
                                PS."Level2",
                                '-',
                                PS."Level3",
                                '-',
                                PS."Level4",
                                '-',
                                PS."Level5"
                            )
                        ) AS "NumberOfLevel5Covered"
                    FROM
                        (
                            SELECT
                                FS."PollingStationId"
                            FROM
                                "FormSubmissions" FS
                                INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                            WHERE
                                FS."ElectionRoundId" = ANY (electionRoundIds)
                                AND F."Status" <> 'Drafted'
                                AND FS."NumberOfQuestionsAnswered" > 0
                            UNION
                            SELECT
                                "PollingStationId"
                            FROM
                                "PollingStationInformation"
                            WHERE
                                "ElectionRoundId" = ANY (electionRoundIds)
                                AND "NumberOfQuestionsAnswered" > 0
                        ) AS T
                        INNER JOIN "PollingStations" PS ON T."PollingStationId" = PS."Id";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of form submissions (non-draft forms, with answers) plus PSI with answers in the given election rounds
                CREATE OR REPLACE FUNCTION "GetPlatformStatisticsFormSubmissions"(electionRoundIds UUID[])
                RETURNS BIGINT
                AS $$
                    SELECT
                        (
                            SELECT
                                COUNT(1)
                            FROM
                                "FormSubmissions" FS
                                INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                            WHERE
                                FS."ElectionRoundId" = ANY (electionRoundIds)
                                AND FS."NumberOfQuestionsAnswered" > 0
                                AND F."Status" <> 'Drafted'
                        ) + (
                            SELECT
                                COUNT(1)
                            FROM
                                "PollingStationInformation"
                            WHERE
                                "ElectionRoundId" = ANY (electionRoundIds)
                                AND "NumberOfQuestionsAnswered" > 0
                        ) AS "NumberOfQuestionsAnswered";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of questions answered (non-draft form submissions plus PSI) in the given election rounds
                CREATE OR REPLACE FUNCTION "GetPlatformStatisticsQuestionsAnswered"(electionRoundIds UUID[])
                RETURNS BIGINT
                AS $$
                    SELECT
                        (
                            SELECT
                                SUM("NumberOfQuestionsAnswered")
                            FROM
                                "FormSubmissions" FS
                                INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                            WHERE
                                FS."ElectionRoundId" = ANY (electionRoundIds)
                                AND FS."NumberOfQuestionsAnswered" > 0
                                AND F."Status" <> 'Drafted'
                        ) + (
                            SELECT
                                SUM("NumberOfQuestionsAnswered")
                            FROM
                                "PollingStationInformation"
                            WHERE
                                "ElectionRoundId" = ANY (electionRoundIds)
                        ) AS "NumberOfFormSubmissions";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of flagged answers on non-draft form submissions in the given election rounds
                CREATE OR REPLACE FUNCTION "GetPlatformStatisticsFlaggedAnswers"(electionRoundIds UUID[])
                RETURNS BIGINT
                AS $$
                    SELECT
                        SUM("NumberOfFlaggedAnswers")
                    FROM
                        "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    WHERE
                        FS."ElectionRoundId" = ANY (electionRoundIds)
                        AND "NumberOfQuestionsAnswered" > 0
                        AND F."Status" <> 'Drafted';
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Minutes monitoring (sum over PSI) in the given election rounds
                CREATE OR REPLACE FUNCTION "GetPlatformStatisticsMinutesMonitoring"(electionRoundIds UUID[])
                RETURNS DOUBLE PRECISION
                AS $$
                    SELECT
                        SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks"))
                    FROM
                        "PollingStationInformation"
                    WHERE
                        "ElectionRoundId" = ANY (electionRoundIds);
                $$ LANGUAGE sql STABLE;
                """);

            // notifications
            migrationBuilder.Sql(
                """
                -- Ids of the NGO's monitoring observers matching the notification recipient filters (observer, polling station,
                -- form submission, polling station information and quick report filters). The submissions' "QuestionsAnswered" is
                -- 'All'/'Some'/'None', or 'ALL'/'SOME'/'NONE' when upperCaseSubmissionQuestionsAnswered (the recipients page and the
                -- send endpoint always used the upper case values, the recipients count the mixed case ones).
                CREATE OR REPLACE FUNCTION "GetNotificationRecipientObserverIds"(
                    electionRoundId UUID,
                    ngoId UUID,
                    searchText TEXT,
                    formType TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    hasFlaggedAnswers BOOLEAN,
                    submissionsFollowUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    hasNotes BOOLEAN,
                    hasAttachments BOOLEAN,
                    questionsAnswered TEXT,
                    fromDate TEXT,
                    toDate TEXT,
                    hasQuickReports BOOLEAN,
                    quickReportFollowUpStatus TEXT,
                    quickReportIncidentCategory TEXT,
                    upperCaseSubmissionQuestionsAnswered BOOLEAN
                )
                    RETURNS TABLE
                            (
                                "MonitoringObserverId" UUID
                            )
                AS
                $$
                WITH
                    "ObserverPSI" AS (
                        SELECT
                            F."Id" AS "FormId",
                            F."FormType" AS "FormType",
                            MO."Id" AS "MonitoringObserverId",
                            FS."PollingStationId" AS "PollingStationId",
                            FS."FollowUpStatus" AS "FollowUpStatus",
                            FS."LastUpdatedAt" AS "LastModifiedOn",
                            FS."IsCompleted" AS "IsCompleted",
                            CAST(NULL AS BIGINT) AS "MediaFilesCount",
                            CAST(NULL AS BIGINT) AS "NotesCount",
                            (
                                CASE
                                    WHEN FS."NumberOfQuestionsAnswered" = F."NumberOfQuestions" THEN 'All'
                                    WHEN FS."NumberOfQuestionsAnswered" > 0 THEN 'Some'
                                    WHEN FS."NumberOfQuestionsAnswered" = 0 THEN 'None'
                                    END
                                ) "QuestionsAnswered",
                            (
                                CASE
                                    WHEN FS."NumberOfFlaggedAnswers" > 0 THEN TRUE
                                    ELSE FALSE
                                    END
                                ) "HasFlaggedAnswers",
                            CAST(NULL AS UUID) AS "QuickReportId",
                            NULL AS "IncidentCategory",
                            NULL AS "QuickReportFollowUpStatus"
                        FROM
                            "MonitoringObservers" MO
                                INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                                INNER JOIN "PollingStationInformation" FS ON MO."Id" = FS."MonitoringObserverId"
                                INNER JOIN "PollingStationInformationForms" F ON F."ElectionRoundId" = electionRoundId
                        WHERE
                            MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                    ),
                    "ObserversFormSubmissions" AS (
                        SELECT
                            AF."FormId" AS "FormId",
                            AF."FormType" AS "FormType",
                            MO."Id" AS "MonitoringObserverId",
                            FS."PollingStationId" AS "PollingStationId",
                            FS."FollowUpStatus" AS "FollowUpStatus",
                            FS."LastUpdatedAt" AS "LastModifiedOn",
                            FS."IsCompleted" AS "IsCompleted",
                            (
                                SELECT
                                    COUNT(*)
                                FROM
                                    "Attachments" A
                                WHERE
                                    (
                                        (A."FormId" = FS."FormId" AND FS."PollingStationId" = A."PollingStationId") -- backwards compatibility
                                        OR A."SubmissionId" = FS."Id"
                                    )
                                  AND A."MonitoringObserverId" = FS."MonitoringObserverId"
                                  AND A."IsDeleted" = FALSE
                                  AND A."IsCompleted" = TRUE
                            ) AS "MediaFilesCount",
                            (
                                SELECT
                                    COUNT(*)
                                FROM
                                    "Notes" N
                                WHERE
                                    (
                                        (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId") -- backwards compatibility
                                        OR N."SubmissionId" = FS."Id"
                                    )
                                  AND N."MonitoringObserverId" = FS."MonitoringObserverId"
                            ) AS "NotesCount",
                            (
                                CASE
                                    WHEN FS."NumberOfQuestionsAnswered" = F."NumberOfQuestions"
                                        THEN CASE WHEN upperCaseSubmissionQuestionsAnswered THEN 'ALL' ELSE 'All' END
                                    WHEN FS."NumberOfQuestionsAnswered" > 0
                                        THEN CASE WHEN upperCaseSubmissionQuestionsAnswered THEN 'SOME' ELSE 'Some' END
                                    WHEN FS."NumberOfQuestionsAnswered" = 0
                                        THEN CASE WHEN upperCaseSubmissionQuestionsAnswered THEN 'NONE' ELSE 'None' END
                                    END
                                ) "QuestionsAnswered",
                            (
                                CASE
                                    WHEN FS."NumberOfFlaggedAnswers" > 0 THEN TRUE
                                    ELSE FALSE
                                    END
                                ) "HasFlaggedAnswers",
                            CAST(NULL AS UUID) AS "QuickReportId",
                            NULL AS "IncidentCategory",
                            NULL AS "QuickReportFollowUpStatus"
                        FROM
                            "MonitoringObservers" MO
                                INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                                INNER JOIN "FormSubmissions" FS ON MO."Id" = FS."MonitoringObserverId"
                                INNER JOIN "GetAvailableForms" (electionRoundId, ngoId, 'Coalition') AF ON AF."FormId" = FS."FormId"
                                INNER JOIN "Forms" F ON F."Id" = AF."FormId"
                        WHERE
                            MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                    ),
                    "ObserversQuickReports" AS (
                        SELECT
                            CAST(NULL AS UUID) AS "FormId",
                            NULL AS "FormType",
                            MO."Id" AS "MonitoringObserverId",
                            QR."PollingStationId" AS "PollingStationId",
                            NULL AS "FollowUpStatus",
                            QR."LastUpdatedAt" AS "LastModifiedOn",
                            CAST(NULL AS BOOLEAN) AS "IsCompleted",
                            CAST(NULL AS BIGINT) AS "MediaFilesCount",
                            CAST(NULL AS BIGINT) AS "NotesCount",
                            NULL AS "QuestionsAnswered",
                            CAST(NULL AS BOOLEAN) AS "HasFlaggedAnswers",
                            QR."Id" AS "QuickReportId",
                            QR."IncidentCategory" AS "IncidentCategory",
                            QR."FollowUpStatus" AS "QuickReportFollowUpStatus"
                        FROM
                            "MonitoringObservers" MO
                                INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                                INNER JOIN "QuickReports" QR ON MO."Id" = QR."MonitoringObserverId"
                        WHERE
                            MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                    ),
                    "ObserversActivity" AS (
                        SELECT * FROM "ObserversFormSubmissions"
                        UNION ALL
                        SELECT * FROM "ObserversQuickReports"
                        UNION ALL
                        SELECT * FROM "ObserverPSI"
                    )
                SELECT DISTINCT
                    MO."Id"
                FROM
                    "MonitoringObservers" MO
                        LEFT JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                        INNER JOIN "MonitoringNgos" MN ON MO."MonitoringNgoId" = MN."Id"
                        LEFT JOIN "ObserversActivity" OA ON MO."Id" = OA."MonitoringObserverId"
                        LEFT JOIN "PollingStations" PS ON OA."PollingStationId" = PS."Id"
                WHERE
                    MN."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                  AND (
                    searchText IS NULL
                        OR searchText = ''
                        OR U."DisplayName" ILIKE searchText
                        OR U."Email" ILIKE searchText
                        OR U."PhoneNumber" ILIKE searchText
                        OR MO."Id"::TEXT ILIKE searchText
                    )
                  AND (
                    tagsFilter IS NULL
                        OR CARDINALITY(tagsFilter) = 0
                        OR MO."Tags" && tagsFilter
                    )
                  AND (
                    monitoringObserverStatus IS NULL
                        OR MO."Status" = monitoringObserverStatus
                    )
                  AND (
                    formType IS NULL
                        OR OA."FormType" = formType
                    )
                  AND (
                    level1 IS NULL
                        OR PS."Level1" = level1
                    )
                  AND (
                    level2 IS NULL
                        OR PS."Level2" = level2
                    )
                  AND (
                    level3 IS NULL
                        OR PS."Level3" = level3
                    )
                  AND (
                    level4 IS NULL
                        OR PS."Level4" = level4
                    )
                  AND (
                    level5 IS NULL
                        OR PS."Level5" = level5
                    )
                  AND (
                    pollingStationNumber IS NULL
                        OR PS."Number" = pollingStationNumber
                    )
                  AND (
                    hasFlaggedAnswers IS NULL
                        OR OA."HasFlaggedAnswers" = hasFlaggedAnswers
                    )
                  AND (
                    submissionsFollowUpStatus IS NULL
                        OR OA."FollowUpStatus" = submissionsFollowUpStatus
                    )
                  AND (
                    formId IS NULL
                        OR OA."FormId" = formId
                    )
                  AND (
                    questionsAnswered IS NULL
                        OR OA."QuestionsAnswered" = questionsAnswered
                    )
                  AND (
                    hasAttachments IS NULL
                        OR (
                        hasAttachments = TRUE
                            AND OA."MediaFilesCount" > 0
                        )
                        OR (
                        hasAttachments = FALSE
                            AND OA."MediaFilesCount" = 0
                        )
                    )
                  AND (
                    hasNotes IS NULL
                        OR (
                        OA."NotesCount" = 0
                            AND hasNotes = FALSE
                        )
                        OR (
                        OA."NotesCount" > 0
                            AND hasNotes = TRUE
                        )
                    )
                  AND (
                    fromDate IS NULL
                        OR OA."LastModifiedOn" >= fromDate::TIMESTAMP
                    )
                  AND (
                    toDate IS NULL
                        OR OA."LastModifiedOn" <= toDate::TIMESTAMP
                    )
                  AND (
                    hasQuickReports IS NULL
                        OR (
                        hasQuickReports = TRUE
                            AND OA."QuickReportId" IS NOT NULL
                        )
                        OR (
                        hasQuickReports = FALSE
                            AND OA."QuickReportId" IS NULL
                        )
                    )
                  AND (
                    quickReportFollowUpStatus IS NULL
                        OR OA."QuickReportFollowUpStatus" = quickReportFollowUpStatus
                    )
                  AND (
                    quickReportIncidentCategory IS NULL
                        OR OA."IncidentCategory" = quickReportIncidentCategory
                    )
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The NGO's monitoring observers matching the notification recipient filters, one row per observer (unordered).
                CREATE OR REPLACE FUNCTION "GetNotificationRecipients"(
                    electionRoundId UUID,
                    ngoId UUID,
                    searchText TEXT,
                    formType TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    hasFlaggedAnswers BOOLEAN,
                    submissionsFollowUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    hasNotes BOOLEAN,
                    hasAttachments BOOLEAN,
                    questionsAnswered TEXT,
                    fromDate TEXT,
                    toDate TEXT,
                    hasQuickReports BOOLEAN,
                    quickReportFollowUpStatus TEXT,
                    quickReportIncidentCategory TEXT
                )
                    RETURNS TABLE
                            (
                                "MonitoringObserverId" UUID,
                                "ObserverName"         TEXT,
                                "PhoneNumber"          TEXT,
                                "Email"                TEXT,
                                "Tags"                 TEXT[],
                                "Status"               TEXT
                            )
                AS
                $$
                SELECT
                    MO."Id",
                    U."DisplayName",
                    U."PhoneNumber",
                    U."Email"::TEXT,
                    MO."Tags",
                    MO."Status"
                FROM
                    "GetNotificationRecipientObserverIds"(electionRoundId, ngoId, searchText, formType, level1, level2, level3,
                                                          level4, level5, pollingStationNumber, hasFlaggedAnswers,
                                                          submissionsFollowUpStatus, tagsFilter, monitoringObserverStatus, formId,
                                                          hasNotes, hasAttachments, questionsAnswered, fromDate, toDate,
                                                          hasQuickReports, quickReportFollowUpStatus, quickReportIncidentCategory,
                                                          TRUE) R
                        INNER JOIN "MonitoringObservers" MO ON MO."Id" = R."MonitoringObserverId"
                        LEFT JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The push notification tokens of the NGO's monitoring observers matching the notification recipient filters: one row
                -- per observer and token, with a NULL token for observers without one.
                CREATE OR REPLACE FUNCTION "GetNotificationRecipientTokens"(
                    electionRoundId UUID,
                    ngoId UUID,
                    searchText TEXT,
                    formType TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    hasFlaggedAnswers BOOLEAN,
                    submissionsFollowUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    hasNotes BOOLEAN,
                    hasAttachments BOOLEAN,
                    questionsAnswered TEXT,
                    fromDate TEXT,
                    toDate TEXT,
                    hasQuickReports BOOLEAN,
                    quickReportFollowUpStatus TEXT,
                    quickReportIncidentCategory TEXT
                )
                    RETURNS TABLE
                            (
                                "MonitoringObserverId" UUID,
                                "Token"                TEXT
                            )
                AS
                $$
                SELECT DISTINCT
                    MO."Id",
                    NT."Token"::TEXT
                FROM
                    "GetNotificationRecipientObserverIds"(electionRoundId, ngoId, searchText, formType, level1, level2, level3,
                                                          level4, level5, pollingStationNumber, hasFlaggedAnswers,
                                                          submissionsFollowUpStatus, tagsFilter, monitoringObserverStatus, formId,
                                                          hasNotes, hasAttachments, questionsAnswered, fromDate, toDate,
                                                          hasQuickReports, quickReportFollowUpStatus, quickReportIncidentCategory,
                                                          TRUE) R
                        INNER JOIN "MonitoringObservers" MO ON MO."Id" = R."MonitoringObserverId"
                        LEFT JOIN "NotificationTokens" NT ON NT."ObserverId" = MO."ObserverId"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Number of notifications sent by the NGO's admins in the election round.
                CREATE OR REPLACE FUNCTION "CountNgoSentNotifications"(
                    electionRoundId UUID,
                    ngoId UUID
                )
                    RETURNS BIGINT
                AS
                $$
                SELECT
                    COUNT(DISTINCT N."Id")
                FROM
                    "MonitoringNgos" MN
                    INNER JOIN "NgoAdmins" NA ON NA."NgoId" = MN."NgoId"
                    INNER JOIN "Notifications" N ON N."SenderId" = NA."Id"
                WHERE
                    MN."NgoId" = ngoId
                    AND N."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Notifications sent by the NGO's admins in the election round, with their sender and targeted/read observer counts
                -- (unordered).
                CREATE OR REPLACE FUNCTION "GetNgoSentNotifications"(
                    electionRoundId UUID,
                    ngoId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"                        UUID,
                                "Title"                     TEXT,
                                "Body"                      TEXT,
                                "SentAt"                    TIMESTAMP WITH TIME ZONE,
                                "Sender"                    TEXT,
                                "NumberOfTargetedObservers" BIGINT,
                                "NumberOfReadNotifications" BIGINT
                            )
                AS
                $$
                SELECT
                    N."Id",
                    N."Title"::TEXT,
                    N."Body",
                    N."CreatedOn",
                    U."DisplayName",
                    COUNT(MON."NotificationId"),
                    SUM(CASE WHEN MON."IsRead" = TRUE THEN 1 ELSE 0 END)
                FROM
                    "Notifications" N
                    INNER JOIN "NgoAdmins" NA ON N."SenderId" = NA."Id"
                    INNER JOIN "MonitoringNgos" MN ON NA."NgoId" = MN."NgoId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = NA."Id"
                    LEFT JOIN "MonitoringObserverNotification" MON ON MON."NotificationId" = N."Id"
                WHERE
                    MN."NgoId" = ngoId
                    AND N."ElectionRoundId" = electionRoundId
                    AND MN."ElectionRoundId" = electionRoundId
                GROUP BY
                    N."Id", N."Title", N."Body", N."CreatedOn", U."DisplayName"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- A notification sent by the NGO's admins in the election round, with its receivers and whether they read it.
                CREATE OR REPLACE FUNCTION "GetNgoNotificationDetails"(
                    electionRoundId UUID,
                    ngoId UUID,
                    notificationId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"        UUID,
                                "Title"     TEXT,
                                "Body"      TEXT,
                                "SentAt"    TIMESTAMP WITH TIME ZONE,
                                "Sender"    TEXT,
                                "Receivers" JSONB
                            )
                AS
                $$
                SELECT
                    N."Id",
                    N."Title"::TEXT,
                    N."Body",
                    N."CreatedOn",
                    U."DisplayName",
                    (
                        SELECT
                            JSONB_AGG(
                                JSONB_BUILD_OBJECT(
                                    'Id',
                                    "MonitoringObserverId",
                                    'Name',
                                    MOU."DisplayName",
                                    'HasReadNotification',
                                    MON."IsRead"
                                )
                            )
                        FROM
                            "MonitoringObserverNotification" MON
                            INNER JOIN "MonitoringObservers" MO ON MO."Id" = MON."MonitoringObserverId"
                            LEFT JOIN "AspNetUsers" MOU ON MOU."Id" = MO."ObserverId"
                        WHERE
                            MON."NotificationId" = N."Id"
                    )
                FROM
                    "Notifications" N
                    INNER JOIN "NgoAdmins" NA ON N."SenderId" = NA."Id"
                    INNER JOIN "MonitoringNgos" MN ON NA."NgoId" = MN."NgoId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = NA."Id"
                WHERE
                    MN."NgoId" = ngoId
                    AND N."ElectionRoundId" = electionRoundId
                    AND N."Id" = notificationId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Name of the NGO an observer monitors the election round for (at most one row).
                CREATE OR REPLACE FUNCTION "GetObserverNotificationsNgoName"(
                    electionRoundId UUID,
                    observerId UUID
                )
                    RETURNS TABLE
                            (
                                "NgoName" TEXT
                            )
                AS
                $$
                SELECT
                    NGO."Name"::TEXT
                FROM
                    "MonitoringObservers" MO
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" NGO ON NGO."Id" = MN."NgoId"
                WHERE
                    MO."ObserverId" = observerId
                    AND MO."ElectionRoundId" = electionRoundId
                FETCH FIRST
                    1 ROWS ONLY
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Notifications an observer received in the election round, with their sender and read state (unordered).
                CREATE OR REPLACE FUNCTION "GetObserverReceivedNotifications"(
                    electionRoundId UUID,
                    observerId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"     UUID,
                                "Title"  TEXT,
                                "Body"   TEXT,
                                "Sender" TEXT,
                                "SentAt" TIMESTAMP WITH TIME ZONE,
                                "IsRead" BOOLEAN
                            )
                AS
                $$
                SELECT
                    N."Id",
                    N."Title"::TEXT,
                    N."Body",
                    U."DisplayName",
                    N."CreatedOn",
                    MON."IsRead"
                FROM
                    "Observers" O
                    INNER JOIN "MonitoringObservers" MO ON MO."ObserverId" = O."Id"
                    INNER JOIN "MonitoringObserverNotification" MON ON MON."MonitoringObserverId" = MO."Id"
                    INNER JOIN "Notifications" N ON MON."NotificationId" = N."Id"
                    LEFT JOIN "AspNetUsers" U on U."Id" =  N."SenderId"
                WHERE
                    O."Id" = observerId
                    AND N."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Citizen notifications sent in the election round when the NGO is its citizen reporting NGO, with their sender
                -- (unordered).
                CREATE OR REPLACE FUNCTION "GetNgoSentCitizenNotifications"(
                    electionRoundId UUID,
                    ngoId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"     UUID,
                                "Title"  TEXT,
                                "Body"   TEXT,
                                "SentAt" TIMESTAMP WITH TIME ZONE,
                                "Sender" TEXT
                            )
                AS
                $$
                SELECT
                    CN."Id",
                    CN."Title"::TEXT,
                    CN."Body",
                    CN."CreatedOn",
                    U."DisplayName"
                FROM
                    "CitizenNotifications" CN
                    INNER JOIN "ElectionRounds" ER ON CN."ElectionRoundId" = ER."Id"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = CN."SenderId"
                WHERE
                    MN."NgoId" = ngoId
                    AND CN."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- A citizen notification sent in the election round by one of the NGO's admins, with its sender.
                CREATE OR REPLACE FUNCTION "GetNgoCitizenNotification"(
                    electionRoundId UUID,
                    ngoId UUID,
                    notificationId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"     UUID,
                                "Title"  TEXT,
                                "Body"   TEXT,
                                "SentAt" TIMESTAMP WITH TIME ZONE,
                                "Sender" TEXT
                            )
                AS
                $$
                SELECT
                    CN."Id",
                    CN."Title"::TEXT,
                    CN."Body",
                    CN."CreatedOn",
                    U."DisplayName"
                FROM
                    "CitizenNotifications" CN
                    INNER JOIN "NgoAdmins" NA ON CN."SenderId" = NA."Id"
                    INNER JOIN "MonitoringNgos" MN ON NA."NgoId" = MN."NgoId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = NA."Id"
                WHERE
                    MN."NgoId" = ngoId
                    AND CN."ElectionRoundId" = electionRoundId
                    AND CN."Id" = notificationId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Name of the election round's citizen reporting NGO (at most one row).
                CREATE OR REPLACE FUNCTION "GetCitizenNotificationsNgoName"(
                    electionRoundId UUID
                )
                    RETURNS TABLE
                            (
                                "Name" TEXT
                            )
                AS
                $$
                SELECT
                    N."Name"::TEXT
                FROM
                    "ElectionRounds" ER
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                WHERE
                    ER."Id" = electionRoundId
                FETCH FIRST
                    1 ROWS ONLY
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Citizen notifications sent in the election round (unordered).
                CREATE OR REPLACE FUNCTION "GetElectionRoundCitizenNotifications"(
                    electionRoundId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"     UUID,
                                "Title"  TEXT,
                                "Body"   TEXT,
                                "SentAt" TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT
                    CN."Id",
                    CN."Title"::TEXT,
                    CN."Body",
                    CN."CreatedOn"
                FROM
                    "CitizenNotifications" CN
                WHERE
                    CN."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Observer guides an NGO admin sees in the election round (its own guides and the ones shared with it through its
                -- coalition), with whether the NGO owns them and, for owned guides, the coalition members they are shared with.
                -- All of them when guideId is NULL, otherwise only that guide.
                CREATE OR REPLACE FUNCTION "GetNgoObserverGuides"(
                    electionRoundId UUID,
                    ngoId UUID,
                    guideId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"               UUID,
                                "Title"            TEXT,
                                "FileName"         TEXT,
                                "MimeType"         TEXT,
                                "GuideType"        TEXT,
                                "Text"             TEXT,
                                "WebsiteUrl"       TEXT,
                                "CreatedOn"        TIMESTAMP WITH TIME ZONE,
                                "CreatedBy"        TEXT,
                                "IsGuideOwner"     BOOLEAN,
                                "FilePath"         TEXT,
                                "UploadedFileName" TEXT,
                                "GuideAccess"      JSONB
                            )
                AS
                $$
                SELECT
                	G."Id",
                	G."Title"::TEXT,
                	G."FileName"::TEXT,
                	G."MimeType"::TEXT,
                	G."GuideType",
                	G."Text",
                	G."WebsiteUrl"::TEXT,
                	G."CreatedOn",
                	G."CreatedBy",
                	G."IsGuideOwner",
                	G."FilePath"::TEXT,
                	G."UploadedFileName"::TEXT,
                	CASE
                		WHEN G."IsGuideOwner" THEN COALESCE(
                			(
                				SELECT
                					JSONB_AGG(
                						JSONB_BUILD_OBJECT('NgoId', N."Id", 'Name', N."Name")
                					)
                				FROM
                					"CoalitionGuideAccess" CFA
                					INNER JOIN "Coalitions" C ON C."Id" = CFA."CoalitionId"
                					INNER JOIN "MonitoringNgos" MN ON CFA."MonitoringNgoId" = MN."Id"
                					INNER JOIN "Ngos" N ON MN."NgoId" = N."Id"
                				WHERE
                					C."ElectionRoundId" = electionRoundId
                					AND CFA."GuideId" = G."Id"
                			),
                			'[]'::JSONB
                		)
                		ELSE '[]'::JSONB
                	END
                FROM
                	(
                		SELECT
                			G."Id",
                			G."Title",
                			G."FileName",
                			G."MimeType",
                			G."GuideType",
                			G."Text",
                			G."WebsiteUrl",
                			COALESCE(G."LastModifiedOn", G."CreatedOn") AS "CreatedOn",
                			COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "CreatedBy",
                			EXISTS (
                				SELECT
                					1
                				FROM
                					"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                				WHERE
                					"MonitoringNgoId" = G."MonitoringNgoId"
                			) AS "IsGuideOwner",
                			G."FilePath",
                			G."UploadedFileName"
                		FROM
                			"CoalitionGuideAccess" CGA
                			INNER JOIN "Coalitions" C ON CGA."CoalitionId" = C."Id"
                			INNER JOIN "ObserversGuides" G ON CGA."GuideId" = G."Id"
                			LEFT JOIN "AspNetUsers" CREATOR ON G."CreatedBy" = CREATOR."Id"
                			LEFT JOIN "AspNetUsers" UPDATER ON G."LastModifiedBy" = UPDATER."Id"
                		WHERE
                			CGA."MonitoringNgoId" = (
                				SELECT
                					"MonitoringNgoId"
                				FROM
                					"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                			)
                			AND (guideId IS NULL OR G."Id" = guideId)
                			AND G."IsDeleted" = false
                			AND C."ElectionRoundId" = electionRoundId
                			AND (
                				(
                					SELECT
                						"CoalitionId" IS NOT NULL
                					FROM
                						"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                				)
                				OR (
                					SELECT
                						"IsCoalitionLeader"
                					FROM
                						"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                				)
                			)
                		UNION
                		SELECT
                			G."Id",
                			G."Title",
                			G."FileName",
                			G."MimeType",
                			G."GuideType",
                			G."Text",
                			G."WebsiteUrl",
                			COALESCE(G."LastModifiedOn", G."CreatedOn") AS "CreatedOn",
                			COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "CreatedBy",
                			TRUE AS "IsGuideOwner",
                			G."FilePath",
                			G."UploadedFileName"
                		FROM
                			"ObserversGuides" G
                			INNER JOIN "MonitoringNgos" MN ON G."MonitoringNgoId" = MN."Id"
                			LEFT JOIN "AspNetUsers" CREATOR ON G."CreatedBy" = CREATOR."Id"
                			LEFT JOIN "AspNetUsers" UPDATER ON G."LastModifiedBy" = UPDATER."Id"
                		WHERE
                			MN."ElectionRoundId" = electionRoundId
                			AND (guideId IS NULL OR G."Id" = guideId)
                			AND G."IsDeleted" = false
                			AND G."MonitoringNgoId" = (
                				SELECT
                					"MonitoringNgoId"
                				FROM
                					"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                			)
                	) G
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Observer guides an observer of the NGO sees in the election round: the ones shared with the NGO through its
                -- coalition, or the NGO's own guides when it is not in a coalition.
                CREATE OR REPLACE FUNCTION "GetObserverGuidesForObserver"(
                    electionRoundId UUID,
                    ngoId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"               UUID,
                                "Title"            TEXT,
                                "FileName"         TEXT,
                                "MimeType"         TEXT,
                                "GuideType"        TEXT,
                                "Text"             TEXT,
                                "WebsiteUrl"       TEXT,
                                "FilePath"         TEXT,
                                "UploadedFileName" TEXT,
                                "CreatedOn"        TIMESTAMP WITH TIME ZONE,
                                "CreatedBy"        TEXT
                            )
                AS
                $$
                SELECT
                	G."Id",
                	G."Title"::TEXT,
                	G."FileName"::TEXT,
                	G."MimeType"::TEXT,
                	G."GuideType",
                	G."Text",
                	G."WebsiteUrl"::TEXT,
                	G."FilePath"::TEXT,
                	G."UploadedFileName"::TEXT,
                	G."CreatedOn",
                	G."CreatedBy"::TEXT
                FROM
                	(
                		SELECT
                			G."Id",
                			G."Title",
                			G."FileName",
                			G."MimeType",
                			G."GuideType",
                			G."Text",
                			G."WebsiteUrl",
                			G."FilePath",
                			G."UploadedFileName",
                			COALESCE(G."LastModifiedOn", G."CreatedOn") AS "CreatedOn",
                			n."Name" AS "CreatedBy"
                		FROM
                			"CoalitionGuideAccess" CGA
                			INNER JOIN "Coalitions" C ON CGA."CoalitionId" = C."Id"
                			INNER JOIN "ObserversGuides" G ON CGA."GuideId" = G."Id"
                			INNER JOIN "MonitoringNgos" mn on g."MonitoringNgoId" = mn."Id"
                			INNER JOIN "Ngos" n on n."Id" = mn."NgoId"
                		WHERE
                			CGA."MonitoringNgoId" = (
                				SELECT
                					"MonitoringNgoId"
                				FROM
                					"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                			)
                			AND g."IsDeleted" = false
                			AND C."ElectionRoundId" = electionRoundId
                			AND (
                				(
                					SELECT
                						"CoalitionId" IS NOT NULL
                					FROM
                						"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                				)
                				OR (
                					SELECT
                						"IsCoalitionLeader"
                					FROM
                						"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                				)
                			)
                		UNION
                		SELECT
                			G."Id",
                			G."Title",
                			G."FileName",
                			G."MimeType",
                			G."GuideType",
                			G."Text",
                			G."WebsiteUrl",
                			G."FilePath",
                			G."UploadedFileName",
                			COALESCE(G."LastModifiedOn", G."CreatedOn") AS "CreatedOn",
                			n."Name" AS "CreatedBy"
                		FROM
                			"ObserversGuides" G
                			INNER JOIN "MonitoringNgos" MN ON G."MonitoringNgoId" = MN."Id"
                			INNER JOIN "Ngos" n on n."Id" = mn."NgoId"
                		WHERE
                			MN."ElectionRoundId" = electionRoundId
                			AND g."IsDeleted" = false
                			AND G."MonitoringNgoId" = (
                				SELECT
                					"MonitoringNgoId"
                				FROM
                					"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                			)
                			AND (
                				SELECT
                					"CoalitionId" IS NULL
                				FROM
                					"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                			)
                	) G
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- An observer guide shared with the observer's NGO through its coalition in the election round, with whether the NGO
                -- owns it. Deleted guides are included.
                CREATE OR REPLACE FUNCTION "GetObserverGuideForObserver"(
                    electionRoundId UUID,
                    ngoId UUID,
                    guideId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"               UUID,
                                "Title"            TEXT,
                                "FileName"         TEXT,
                                "MimeType"         TEXT,
                                "GuideType"        TEXT,
                                "Text"             TEXT,
                                "WebsiteUrl"       TEXT,
                                "FilePath"         TEXT,
                                "UploadedFileName" TEXT,
                                "LastModifiedOn"   TIMESTAMP WITH TIME ZONE,
                                "LastModifiedBy"   TEXT,
                                "IsGuideOwner"     BOOLEAN
                            )
                AS
                $$
                SELECT
                	G."Id",
                	G."Title"::TEXT,
                	G."FileName"::TEXT,
                	G."MimeType"::TEXT,
                	G."GuideType",
                	G."Text",
                	G."WebsiteUrl"::TEXT,
                	G."FilePath"::TEXT,
                	G."UploadedFileName"::TEXT,
                	COALESCE(G."LastModifiedOn", G."CreatedOn"),
                	n."Name"::TEXT,
                	EXISTS (
                		SELECT
                			1
                		FROM
                			"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                		WHERE
                			"MonitoringNgoId" = G."MonitoringNgoId"
                	)
                FROM
                	"CoalitionGuideAccess" CGA
                	INNER JOIN "Coalitions" C ON CGA."CoalitionId" = C."Id"
                	INNER JOIN "ObserversGuides" G ON CGA."GuideId" = G."Id"
                	INNER JOIN "MonitoringNgos" mn on g."MonitoringNgoId" = mn."Id"
                	INNER JOIN "Ngos" n on n."Id" = mn."NgoId"
                WHERE
                	CGA."MonitoringNgoId" = (
                		SELECT
                			"MonitoringNgoId"
                		FROM
                			"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                	)
                	AND G."Id" = guideId
                	AND C."ElectionRoundId" = electionRoundId
                	AND (
                		(
                			SELECT
                				"CoalitionId" IS NOT NULL
                			FROM
                				"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                		)
                		OR (
                			SELECT
                				"IsCoalitionLeader"
                			FROM
                				"GetMonitoringNgoDetails" (electionRoundId, ngoId)
                		)
                	)
                $$ LANGUAGE sql STABLE;
                """);

            // forms-observers
            migrationBuilder.Sql(
                """
                -- The forms the NGO sees in an election round (its own forms and the forms its coalition shares with it), matching the
                -- optional search (an ILIKE pattern; NULL or '' matches all), form type and status filters. FormAccess lists the NGOs a
                -- form is shared with, only for forms the NGO owns. Unordered.
                CREATE OR REPLACE FUNCTION "GetNgoFormsList"(
                    electionRoundId UUID,
                    ngoId UUID,
                    searchText TEXT,
                    formType TEXT,
                    formStatus TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "Code"                       TEXT,
                                "Name"                       JSONB,
                                "Description"                JSONB,
                                "DefaultLanguage"            TEXT,
                                "Languages"                  TEXT[],
                                "Status"                     TEXT,
                                "FormType"                   TEXT,
                                "NumberOfQuestions"          INTEGER,
                                "LanguagesTranslationStatus" JSONB,
                                "Icon"                       TEXT,
                                "LastModifiedOn"             TIMESTAMP WITH TIME ZONE,
                                "LastModifiedBy"             TEXT,
                                "IsFormOwner"                BOOLEAN,
                                "FormAccess"                 JSONB
                            )
                AS
                $$
                WITH
                    "MonitoringNgoData" AS (
                        SELECT
                            MN."ElectionRoundId",
                            MN."Id" AS "MonitoringNgoId",
                            -- Check if MonitoringNgo is a coalition leader
                            EXISTS (
                                SELECT
                                    1
                                FROM
                                    "CoalitionMemberships" CM
                                        JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                WHERE
                                    CM."MonitoringNgoId" = MN."Id"
                                  AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                  AND C."LeaderId" = MN."Id"
                            ) AS "IsCoalitionLeader",
                            -- Check if MonitoringNgo is in a coalition
                            (
                                SELECT
                                    COUNT(1)
                                FROM
                                    "CoalitionMemberships" CM
                                WHERE
                                    CM."MonitoringNgoId" = MN."Id"
                                  AND CM."ElectionRoundId" = MN."ElectionRoundId"
                            ) > 0 AS "IsInACoalition"
                        FROM
                            "MonitoringNgos" MN
                        WHERE
                            MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                        LIMIT
                            1
                    )
                SELECT
                    F."Id",
                    F."Code"::text,
                    F."Name",
                    F."Description",
                    F."DefaultLanguage"::text,
                    F."Languages",
                    F."Status",
                    F."FormType",
                    F."NumberOfQuestions",
                    F."LanguagesTranslationStatus",
                    F."Icon",
                    F."LastModifiedOn",
                    F."LastModifiedBy",
                    F."IsFormOwner",
                    CASE WHEN F."IsFormOwner" THEN COALESCE(
                            (SELECT JSONB_AGG(
                                            JSONB_BUILD_OBJECT(
                                                    'NgoId',
                                                    N."Id",
                                                    'Name',
                                                    N."Name"
                                            )
                                    )
                             FROM "CoalitionFormAccess" CFA
                                      INNER JOIN "Coalitions" C ON C."Id" = CFA."CoalitionId"
                                      INNER JOIN "MonitoringNgos" MN ON CFA."MonitoringNgoId" = MN."Id"
                                      INNER JOIN "Ngos" N ON MN."NgoId" = N."Id"
                             WHERE C."ElectionRoundId" = electionRoundId
                               AND CFA."FormId" = F."Id"),
                            '[]'::JSONB) ELSE '[]'::JSONB END
                FROM
                    (
                        SELECT
                            F."Id",
                            F."Code",
                            F."Name",
                            F."Description",
                            F."DefaultLanguage",
                            F."Languages",
                            F."Status",
                            F."FormType",
                            F."NumberOfQuestions",
                            F."LanguagesTranslationStatus",
                            F."Icon",
                            COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                            COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                            EXISTS (
                                SELECT 1
                                FROM "MonitoringNgoData"
                                WHERE "MonitoringNgoId" = F."MonitoringNgoId"
                            ) AS "IsFormOwner"
                        FROM
                            "CoalitionFormAccess" CFA
                                INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                                INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                                LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                        WHERE
                            CFA."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                          AND C."ElectionRoundId" = electionRoundId
                          AND (
                            (SELECT "IsInACoalition" FROM "MonitoringNgoData")
                            OR (SELECT "IsCoalitionLeader" FROM "MonitoringNgoData")
                            )
                          AND (
                            searchText IS NULL
                                OR searchText = ''
                                OR F."Code" ILIKE searchText
                                OR F."Name" ->> F."DefaultLanguage" ILIKE searchText
                                OR F."Description" ->> F."DefaultLanguage" ILIKE searchText
                                OR F."Id"::TEXT ILIKE searchText
                            )
                          AND (formType IS NULL OR F."FormType" = formType)
                          AND (formStatus IS NULL OR F."Status" = formStatus)
                        UNION
                        SELECT
                            F."Id",
                            F."Code",
                            F."Name",
                            F."Description",
                            F."DefaultLanguage",
                            F."Languages",
                            F."Status",
                            F."FormType",
                            F."NumberOfQuestions",
                            F."LanguagesTranslationStatus",
                            F."Icon",
                            COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                            COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                            TRUE AS "IsFormOwner"
                        FROM
                            "Forms" F
                                LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                        WHERE
                            F."ElectionRoundId" = electionRoundId
                          AND F."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                          AND (
                            searchText IS NULL
                                OR searchText = ''
                                OR F."Code" ILIKE searchText
                                OR F."Name" ->> F."DefaultLanguage" ILIKE searchText
                                OR F."Description" ->> F."DefaultLanguage" ILIKE searchText
                                OR F."Id"::TEXT ILIKE searchText
                            )
                          AND (formType IS NULL OR F."FormType" = formType)
                          AND (formStatus IS NULL OR F."Status" = formStatus)
                    ) F
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- One form the NGO can see in an election round (its own, shared with it by its coalition, or the round's polling
                -- station information form), with its questions. At most one row.
                CREATE OR REPLACE FUNCTION "GetNgoFormDetails"(
                    electionRoundId UUID,
                    ngoId UUID,
                    formId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "Code"                       TEXT,
                                "Name"                       JSONB,
                                "Description"                JSONB,
                                "DefaultLanguage"            TEXT,
                                "Languages"                  TEXT[],
                                "Status"                     TEXT,
                                "FormType"                   TEXT,
                                "NumberOfQuestions"          INTEGER,
                                "LanguagesTranslationStatus" JSONB,
                                "Icon"                       TEXT,
                                "DisplayOrder"               INTEGER,
                                "Questions"                  JSONB,
                                "LastModifiedOn"             TIMESTAMP WITH TIME ZONE,
                                "LastModifiedBy"             TEXT,
                                "IsFormOwner"                BOOLEAN
                            )
                AS
                $$
                WITH
                    "MonitoringNgoData" AS (
                        SELECT
                            MN."ElectionRoundId",
                            MN."Id" AS "MonitoringNgoId",
                            EXISTS (
                                SELECT 1
                                FROM "CoalitionMemberships" CM
                                         JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                WHERE CM."MonitoringNgoId" = MN."Id"
                                  AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                  AND C."LeaderId" = MN."Id"
                            ) AS "IsCoalitionLeader",
                            (
                                SELECT COUNT(1)
                                FROM "CoalitionMemberships" CM
                                WHERE CM."MonitoringNgoId" = MN."Id"
                                  AND CM."ElectionRoundId" = MN."ElectionRoundId"
                            ) > 0 AS "IsInACoalition"
                        FROM "MonitoringNgos" MN
                        WHERE MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                        LIMIT 1
                    )
                SELECT
                    F."Id",
                    F."Code"::text,
                    F."Name",
                    F."Description",
                    F."DefaultLanguage"::text,
                    F."Languages",
                    F."Status",
                    F."FormType",
                    F."NumberOfQuestions",
                    F."LanguagesTranslationStatus",
                    F."Icon",
                    F."DisplayOrder",
                    F."Questions",
                    F."LastModifiedOn",
                    F."LastModifiedBy",
                    F."IsFormOwner"
                FROM (
                         SELECT
                             F."Id",
                             F."Code",
                             F."Name",
                             F."Description",
                             F."DefaultLanguage",
                             F."Languages",
                             F."Status",
                             F."FormType",
                             F."NumberOfQuestions",
                             F."LanguagesTranslationStatus",
                             F."Icon",
                             F."DisplayOrder",
                             F."Questions",
                             COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                             COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                             EXISTS (
                                 SELECT 1
                                 FROM "MonitoringNgoData"
                                 WHERE "MonitoringNgoId" = F."MonitoringNgoId"
                             ) AS "IsFormOwner"
                         FROM "CoalitionFormAccess" CFA
                                  INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                                  INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                                  LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                  LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                         WHERE CFA."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                           AND C."ElectionRoundId" = electionRoundId
                           AND F."Id" = formId
                           AND (
                             (SELECT "IsInACoalition" FROM "MonitoringNgoData")
                                 OR (SELECT "IsCoalitionLeader" FROM "MonitoringNgoData")
                             )

                         UNION

                         SELECT
                             F."Id",
                             F."Code",
                             F."Name",
                             F."Description",
                             F."DefaultLanguage",
                             F."Languages",
                             F."Status",
                             F."FormType",
                             F."NumberOfQuestions",
                             F."LanguagesTranslationStatus",
                             F."Icon",
                             F."DisplayOrder",
                             F."Questions",
                             COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                             COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                             TRUE AS "IsFormOwner"
                         FROM "Forms" F
                                  LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                  LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                         WHERE F."ElectionRoundId" = electionRoundId
                           AND F."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                           AND F."Id" = formId

                         UNION

                         SELECT
                             PSIF."Id",
                             PSIF."Code",
                             PSIF."Name",
                             PSIF."Description",
                             PSIF."DefaultLanguage",
                             PSIF."Languages",
                             PSIF."Status",
                             PSIF."FormType",
                             PSIF."NumberOfQuestions",
                             PSIF."LanguagesTranslationStatus",
                             NULL AS "Icon",
                             0 AS "DisplayOrder",
                             PSIF."Questions",
                             COALESCE(PSIF."LastModifiedOn", PSIF."CreatedOn") AS "LastModifiedOn",
                             COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                             FALSE AS "IsFormOwner"
                         FROM "PollingStationInformationForms" PSIF
                                  LEFT JOIN "AspNetUsers" CREATOR ON PSIF."CreatedBy" = CREATOR."Id"
                                  LEFT JOIN "AspNetUsers" UPDATER ON PSIF."LastModifiedBy" = UPDATER."Id"
                         WHERE PSIF."ElectionRoundId" = electionRoundId
                           AND PSIF."Id" = formId
                     ) F
                LIMIT 1
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The platform's form templates matching the optional search (an ILIKE pattern; NULL or '' matches all), form type and
                -- status filters, with who last modified them. Unordered.
                CREATE OR REPLACE FUNCTION "GetFormTemplatesList"(
                    searchText TEXT,
                    formType TEXT,
                    formStatus TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "Code"                       TEXT,
                                "Name"                       JSONB,
                                "Description"                JSONB,
                                "DefaultLanguage"            TEXT,
                                "Languages"                  TEXT[],
                                "Status"                     TEXT,
                                "FormType"                   TEXT,
                                "NumberOfQuestions"          INTEGER,
                                "Icon"                       TEXT,
                                "LanguagesTranslationStatus" JSONB,
                                "LastModifiedOn"             TIMESTAMP WITH TIME ZONE,
                                "LastModifiedBy"             TEXT
                            )
                AS
                $$
                SELECT FT."Id",
                       FT."Code"::text,
                       FT."Name",
                       FT."Description",
                       FT."DefaultLanguage"::text,
                       FT."Languages",
                       FT."Status",
                       FT."FormType",
                       FT."NumberOfQuestions",
                       FT."Icon",
                       FT."LanguagesTranslationStatus",
                       COALESCE(FT."LastModifiedOn", FT."CreatedOn"),
                       COALESCE(UPDATER."DisplayName", CREATOR."DisplayName")
                FROM "FormTemplates" FT
                         LEFT JOIN "AspNetUsers" CREATOR ON FT."CreatedBy" = CREATOR."Id"
                         LEFT JOIN "AspNetUsers" UPDATER ON FT."LastModifiedBy" = UPDATER."Id"
                WHERE (
                    searchText IS NULL
                        OR searchText = ''
                        OR FT."Code" ILIKE searchText
                        OR FT."Name" ->> FT."DefaultLanguage" ILIKE searchText
                        OR FT."Description" ->> FT."DefaultLanguage" ILIKE searchText
                        OR FT."Id"::TEXT ILIKE searchText
                    )
                  AND (formType IS NULL OR FT."FormType" = formType)
                  AND (formStatus IS NULL OR FT."Status" = formStatus)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The NGO's monitoring observers in an election round matching the optional search (an ILIKE pattern; NULL or ''
                -- matches all), tags (any of them; NULL or empty matches all) and status filters, with their latest activity in the
                -- round. The invitation token is returned only for pending observers. Unordered.
                CREATE OR REPLACE FUNCTION "GetMonitoringObserversList"(
                    electionRoundId UUID,
                    ngoId UUID,
                    searchText TEXT,
                    tagsFilter TEXT[],
                    observerStatus TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"               UUID,
                                "DisplayName"      TEXT,
                                "FirstName"        TEXT,
                                "LastName"         TEXT,
                                "PhoneNumber"      TEXT,
                                "Email"            TEXT,
                                "Tags"             TEXT[],
                                "Status"           TEXT,
                                "LatestActivityAt" TIMESTAMP WITH TIME ZONE,
                                "InvitationToken"  TEXT
                            )
                AS
                $$
                SELECT
                    MO."Id",
                    U."DisplayName",
                    U."FirstName"::text,
                    U."LastName"::text,
                    U."PhoneNumber",
                    U."Email"::text,
                    MO."Tags",
                    MO."Status",
                    MAX(LATESTACTIVITY."LatestActivityAt"),
                    (CASE WHEN MO."Status" = 'Pending' THEN U."InvitationToken" ELSE NULL END)::text
                FROM
                    "MonitoringObservers" MO
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                    LEFT JOIN (
                        SELECT
                            "MonitoringObserverId",
                            MAX("LatestActivityAt") AS "LatestActivityAt"
                        FROM
                            (
                                SELECT
                                    PSI."MonitoringObserverId",
                                    MAX(PSI."LastUpdatedAt") AS "LatestActivityAt"
                                FROM
                                    "PollingStationInformation" PSI
                                WHERE
                                    PSI."ElectionRoundId" = electionRoundId
                                GROUP BY
                                    PSI."MonitoringObserverId"
                                UNION ALL
                                SELECT
                                    N."MonitoringObserverId",
                                    MAX(N."LastUpdatedAt") AS "LatestActivityAt"
                                FROM
                                    "Notes" N
                                INNER JOIN "MonitoringObservers" MO ON N."MonitoringObserverId" = MO."Id"
                                WHERE
                                    MO."ElectionRoundId" = electionRoundId
                                GROUP BY
                                    N."MonitoringObserverId"
                                UNION ALL
                                SELECT
                                    A."MonitoringObserverId",
                                    MAX(A."LastUpdatedAt") AS "LatestActivityAt"
                                FROM
                                    "Attachments" A
                                INNER JOIN "MonitoringObservers" MO ON A."MonitoringObserverId" = MO."Id"
                                WHERE
                                    MO."ElectionRoundId" = electionRoundId
                                GROUP BY
                                    A."MonitoringObserverId"
                                UNION ALL
                                SELECT
                                    QR."MonitoringObserverId",
                                    MAX(QR."LastUpdatedAt") AS "LatestActivityAt"
                                FROM
                                    "QuickReports" QR
                                WHERE
                                    QR."ElectionRoundId" = electionRoundId
                                GROUP BY
                                    QR."MonitoringObserverId"
                            ) AS LATESTACTIVITYSUBQUERY
                        GROUP BY
                            "MonitoringObserverId"
                    ) AS LATESTACTIVITY ON LATESTACTIVITY."MonitoringObserverId" = MO."Id"
                WHERE
                    MN."ElectionRoundId" = electionRoundId
                    AND MN."NgoId" = ngoId
                    AND (searchText IS NULL
                             OR searchText = ''
                             OR U."DisplayName" ILIKE searchText
                             OR U."Email" ILIKE searchText
                             OR U."PhoneNumber" ILIKE searchText
                             OR MO."Id"::TEXT ILIKE searchText)
                    AND (tagsFilter IS NULL OR cardinality(tagsFilter) = 0 OR MO."Tags" && tagsFilter)
                    AND (observerStatus IS NULL OR MO."Status" = observerStatus)
                GROUP BY
                    MO."Id",
                    U."DisplayName",
                    U."FirstName",
                    U."LastName",
                    U."PhoneNumber",
                    U."Email",
                    MO."Tags",
                    MO."Status",
                    U."InvitationToken"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- One monitoring observer the NGO can see in an election round (its own or, in a coalition, a member's), with its
                -- latest activity in the round. The invitation token is returned only for a pending observer. At most one row.
                CREATE OR REPLACE FUNCTION "GetMonitoringObserverDetails"(
                    electionRoundId UUID,
                    ngoId UUID,
                    monitoringObserverId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"               UUID,
                                "DisplayName"      TEXT,
                                "FirstName"        TEXT,
                                "LastName"         TEXT,
                                "PhoneNumber"      TEXT,
                                "Email"            TEXT,
                                "Tags"             TEXT[],
                                "Status"           TEXT,
                                "IsOwnObserver"    BOOLEAN,
                                "LatestActivityAt" TIMESTAMP WITH TIME ZONE,
                                "InvitationToken"  TEXT
                            )
                AS
                $$
                WITH
                    MONITORINGOBSERVER AS (
                        SELECT
                            AMO."MonitoringObserverId" AS "Id",
                            AMO."DisplayName",
                            AMO."FirstName",
                            AMO."LastName",
                            AMO."PhoneNumber",
                            AMO."Email",
                            AMO."Tags",
                            AMO."Status",
                            AMO."NgoName",
                            AMO."IsOwnObserver"
                        FROM
                            "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO
                        WHERE
                            AMO."MonitoringObserverId" = monitoringObserverId
                        LIMIT 1
                    ),
                    LATESTTIMESTAMPS AS (
                        SELECT
                            MAX(PSI."LastUpdatedAt") AS "LatestActivityAt"
                        FROM
                            "PollingStationInformation" PSI
                        WHERE
                            PSI."ElectionRoundId" = electionRoundId
                          AND PSI."MonitoringObserverId" = (SELECT "Id" FROM MONITORINGOBSERVER)

                        UNION ALL

                        SELECT
                            MAX(N."LastUpdatedAt") AS "LatestActivityAt"
                        FROM
                            "Notes" N
                        WHERE
                            N."MonitoringObserverId" = (SELECT "Id" FROM MONITORINGOBSERVER)

                        UNION ALL

                        SELECT
                            MAX(A."LastUpdatedAt") AS "LatestActivityAt"
                        FROM
                            "Attachments" A
                        INNER JOIN "MonitoringObservers" MO ON A."MonitoringObserverId" = MO."Id"
                        WHERE
                            MO."ElectionRoundId" = electionRoundId
                          AND A."MonitoringObserverId" = (SELECT "Id" FROM MONITORINGOBSERVER)

                        UNION ALL

                        SELECT
                            MAX(QR."LastUpdatedAt") AS "LatestActivityAt"
                        FROM
                            "QuickReports" QR
                        WHERE
                            QR."ElectionRoundId" = electionRoundId
                          AND QR."MonitoringObserverId" = (SELECT "Id" FROM MONITORINGOBSERVER)
                    )
                SELECT
                    MO."Id",
                    MO."DisplayName",
                    MO."FirstName",
                    MO."LastName",
                    MO."PhoneNumber",
                    MO."Email",
                    MO."Tags",
                    MO."Status",
                    MO."IsOwnObserver",
                    MAX(LT."LatestActivityAt"),
                    (CASE WHEN MO."Status" = 'Pending' THEN U."InvitationToken" ELSE NULL END)::text
                FROM
                    MONITORINGOBSERVER MO
                    LEFT JOIN LATESTTIMESTAMPS LT ON TRUE
                    INNER JOIN "MonitoringObservers" M ON M."Id" = MO."Id"
                    INNER JOIN "Observers" O ON O."Id" = M."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                GROUP BY
                    MO."Id",
                    MO."DisplayName",
                    MO."FirstName",
                    MO."LastName",
                    MO."PhoneNumber",
                    MO."Email",
                    MO."Tags",
                    MO."Status",
                    MO."IsOwnObserver",
                    U."InvitationToken"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The NGO's monitoring observers in an election round, with their contact details, tags and status, for the CSV export.
                CREATE OR REPLACE FUNCTION "GetMonitoringObserversExport"(
                    electionRoundId UUID,
                    ngoId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"          UUID,
                                "FirstName"   TEXT,
                                "LastName"    TEXT,
                                "PhoneNumber" TEXT,
                                "Email"       TEXT,
                                "Tags"        TEXT[],
                                "Status"      TEXT
                            )
                AS
                $$
                SELECT
                    MO."Id",
                    U."FirstName"::text,
                    U."LastName"::text,
                    U."PhoneNumber",
                    U."Email"::text,
                    MO."Tags",
                    MO."Status"
                FROM
                    "MonitoringObservers" MO
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE
                    MN."ElectionRoundId" = electionRoundId
                    AND MN."NgoId" = ngoId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The platform's observers matching the optional search (an ILIKE pattern; NULL or '' matches all), account status and
                -- email-verified filters, with the election rounds they monitor (and for which NGO). Unordered.
                CREATE OR REPLACE FUNCTION "GetPlatformObserversList"(
                    searchText TEXT,
                    userStatus TEXT,
                    isEmailVerified BOOLEAN
                )
                    RETURNS TABLE
                            (
                                "Id"                 UUID,
                                "Email"              TEXT,
                                "FirstName"          TEXT,
                                "LastName"           TEXT,
                                "PhoneNumber"        TEXT,
                                "Status"             TEXT,
                                "IsAccountVerified"  BOOLEAN,
                                "MonitoredElections" JSONB
                            )
                AS
                $$
                SELECT
                    U."Id",
                    U."Email"::text,
                    U."FirstName"::text,
                    U."LastName"::text,
                    U."PhoneNumber",
                    U."Status",
                    U."EmailConfirmed",
                    COALESCE((SELECT JSONB_AGG(JSONB_BUILD_OBJECT('Id', ER."Id",
                                                                  'Title', ER."Title",
                                                                  'EnglishTitle', ER."EnglishTitle",
                                                                  'StartDate', ER."StartDate",
                                                                  'Status', ER."Status",
                                                                  'NgoName', N."Name"
                                               ))
                              FROM "MonitoringObservers" MO
                                       LEFT JOIN "ElectionRounds" ER ON ER."Id" = MO."ElectionRoundId"
                                       LEFT JOIN "MonitoringNgos" MN ON MO."MonitoringNgoId" = MN."Id"
                                       LEFT JOIN "Ngos" N ON N."Id" = MN."NgoId"
                              WHERE MO."ObserverId" = O."Id"), '[]'::JSONB)
                FROM "Observers" O
                         LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE (
                    searchText IS NULL
                        OR searchText = ''
                        OR U."Id"::TEXT ILIKE searchText
                        OR U."FirstName" ILIKE searchText
                        OR U."LastName" ILIKE searchText
                        OR U."Email" ILIKE searchText
                        OR U."PhoneNumber" ILIKE searchText
                    )
                  AND (userStatus IS NULL OR U."Status" = userStatus)
                  AND (isEmailVerified IS NULL OR U."EmailConfirmed" = isEmailVerified)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- One observer of the platform, with the election rounds they monitor (and for which NGO). At most one row.
                CREATE OR REPLACE FUNCTION "GetPlatformObserverDetails"(
                    observerId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"                 UUID,
                                "Email"              TEXT,
                                "FirstName"          TEXT,
                                "LastName"           TEXT,
                                "PhoneNumber"        TEXT,
                                "Status"             TEXT,
                                "IsAccountVerified"  BOOLEAN,
                                "MonitoredElections" JSONB
                            )
                AS
                $$
                SELECT
                    U."Id",
                    U."Email"::text,
                    U."FirstName"::text,
                    U."LastName"::text,
                    U."PhoneNumber",
                    U."Status",
                    U."EmailConfirmed",
                    COALESCE((SELECT JSONB_AGG(JSONB_BUILD_OBJECT('Id', ER."Id",
                                                                  'Title', ER."Title",
                                                                  'EnglishTitle', ER."EnglishTitle",
                                                                  'StartDate', ER."StartDate",
                                                                  'Status', ER."Status",
                                                                  'NgoName', N."Name"
                                               ))
                              FROM "MonitoringObservers" MO
                                       LEFT JOIN "ElectionRounds" ER ON ER."Id" = MO."ElectionRoundId"
                                       LEFT JOIN "MonitoringNgos" MN ON MO."MonitoringNgoId" = MN."Id"
                                       LEFT JOIN "Ngos" N ON N."Id" = MN."NgoId"
                              WHERE MO."ObserverId" = O."Id"), '[]'::JSONB)
                FROM "Observers" O
                         LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE O."Id" = observerId
                $$ LANGUAGE sql STABLE;
                """);

            // ngos-rounds-stations
            migrationBuilder.Sql(
                """
                -- The platform admin NGO list: every NGO matching the search text (an ILIKE pattern on name or id) and status, with
                -- its admin, staff and monitored election counts and the start date of its latest election. Null arguments don't filter.
                CREATE OR REPLACE FUNCTION "GetNgoOverviews"(
                    searchText TEXT,
                    ngoStatus TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "Name"                       TEXT,
                                "Status"                     TEXT,
                                "NumberOfNgoAdmins"          BIGINT,
                                "NumberOfStaff"              BIGINT,
                                "NumberOfMonitoredElections" BIGINT,
                                "DateOfLastElection"         DATE
                            )
                AS
                $$
                SELECT N."Id",
                       N."Name"::text,
                       N."Status",
                       (SELECT COUNT(1)
                        FROM "NgoAdmins" NA
                        WHERE NA."NgoId" = N."Id"),
                       (SELECT COUNT(1)
                        FROM "NgoStaff" NS
                        WHERE NS."NgoId" = N."Id"),
                       (SELECT COUNT(1)
                        FROM "MonitoringNgos" MN
                        WHERE MN."NgoId" = N."Id"),
                       (SELECT MAX(ER."StartDate")
                        FROM "MonitoringNgos" MN
                            INNER JOIN "ElectionRounds" ER ON ER."Id" = MN."ElectionRoundId"
                        WHERE MN."NgoId" = N."Id")
                FROM "Ngos" N
                WHERE (searchText IS NULL OR N."Name" ILIKE searchText OR N."Id"::TEXT ILIKE searchText)
                  AND (ngoStatus IS NULL OR N."Status" = ngoStatus)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- One NGO with its admin, staff and monitored election counts, its monitored elections (jsonb array) and the start
                -- date of its latest election. No row when the NGO doesn't exist.
                CREATE OR REPLACE FUNCTION "GetNgoDetails"(
                    ngoId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "Name"                       TEXT,
                                "Status"                     TEXT,
                                "NumberOfNgoAdmins"          BIGINT,
                                "NumberOfStaff"              BIGINT,
                                "NumberOfMonitoredElections" BIGINT,
                                "MonitoredElections"         JSONB,
                                "DateOfLastElection"         DATE
                            )
                AS
                $$
                SELECT N."Id",
                       N."Name"::text,
                       N."Status",
                       (SELECT COUNT(1)
                        FROM "NgoAdmins" NA
                        WHERE NA."NgoId" = N."Id"),
                       (SELECT COUNT(1)
                        FROM "NgoStaff" NS
                        WHERE NS."NgoId" = N."Id"),
                       (SELECT COUNT(1)
                        FROM "MonitoringNgos" MN
                        WHERE MN."NgoId" = N."Id"),
                       COALESCE((SELECT jsonb_agg(jsonb_build_object('Id', ER."Id",
                                                                     'Title', ER."Title",
                                                                     'EnglishTitle', ER."EnglishTitle",
                                                                     'StartDate', ER."StartDate",
                                                                     'Status', ER."Status"))
                                 FROM "MonitoringNgos" MN
                                     INNER JOIN "ElectionRounds" ER ON ER."Id" = MN."ElectionRoundId"
                                 WHERE MN."NgoId" = N."Id"), '[]'::JSONB),
                       (SELECT MAX(ER."StartDate")
                        FROM "MonitoringNgos" MN
                            INNER JOIN "ElectionRounds" ER ON ER."Id" = MN."ElectionRoundId"
                        WHERE MN."NgoId" = N."Id")
                FROM "Ngos" N
                WHERE N."Id" = ngoId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The coalitions of an election round matching the search text (an ILIKE pattern on id or name; null or empty doesn't
                -- filter), with their leader NGO and their members (jsonb array of {Id, Name}).
                CREATE OR REPLACE FUNCTION "GetElectionRoundCoalitions"(
                    electionRoundId UUID,
                    searchText TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"         UUID,
                                "Name"       TEXT,
                                "Members"    JSONB,
                                "LeaderId"   UUID,
                                "LeaderName" TEXT
                            )
                AS
                $$
                SELECT C."Id",
                       C."Name"::text,
                       COALESCE((SELECT JSONB_AGG(JSONB_BUILD_OBJECT('Id', CMN."NgoId", 'Name', CN."Name"))
                                 FROM "CoalitionMemberships" CM
                                     INNER JOIN "MonitoringNgos" CMN ON CM."MonitoringNgoId" = CMN."Id"
                                     INNER JOIN "Ngos" CN ON CMN."NgoId" = CN."Id"
                                 WHERE CM."ElectionRoundId" = electionRoundId
                                   AND CM."CoalitionId" = C."Id"), '[]'::JSONB),
                       N."Id",
                       N."Name"::text
                FROM "Coalitions" C
                    INNER JOIN "MonitoringNgos" MN ON C."LeaderId" = MN."Id"
                    INNER JOIN "Ngos" N ON MN."NgoId" = N."Id"
                WHERE C."ElectionRoundId" = electionRoundId
                  AND (searchText IS NULL
                    OR searchText = ''
                    OR C."Id"::TEXT ILIKE searchText
                    OR C."Name" ILIKE searchText)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The platform admin election round list: every election round matching the search text (an ILIKE pattern on title,
                -- English title or id), status and country, with its country and its monitoring NGOs (jsonb array of {Id, Name, Status}).
                -- Null arguments don't filter.
                CREATE OR REPLACE FUNCTION "GetElectionRoundOverviews"(
                    searchText TEXT,
                    electionRoundStatus TEXT,
                    countryId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"                 UUID,
                                "Title"              TEXT,
                                "EnglishTitle"       TEXT,
                                "StartDate"          DATE,
                                "Status"             TEXT,
                                "CreatedOn"          TIMESTAMP WITH TIME ZONE,
                                "LastModifiedOn"     TIMESTAMP WITH TIME ZONE,
                                "CountryId"          UUID,
                                "CountryIso2"        TEXT,
                                "CountryIso3"        TEXT,
                                "CountryName"        TEXT,
                                "CountryFullName"    TEXT,
                                "CountryNumericCode" TEXT,
                                "MonitoringNgos"     JSONB
                            )
                AS
                $$
                SELECT ER."Id",
                       ER."Title"::text,
                       ER."EnglishTitle"::text,
                       ER."StartDate",
                       ER."Status",
                       ER."CreatedOn",
                       ER."LastModifiedOn",
                       ER."CountryId",
                       C."Iso2"::text,
                       C."Iso3"::text,
                       C."Name"::text,
                       C."FullName"::text,
                       C."NumericCode"::text,
                       COALESCE((SELECT jsonb_agg(jsonb_build_object('Id', MN."Id", 'Name', N."Name", 'Status', N."Status"))
                                 FROM "MonitoringNgos" MN
                                     INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                                 WHERE MN."ElectionRoundId" = ER."Id"), '[]'::JSONB)
                FROM "ElectionRounds" ER
                    INNER JOIN "Countries" C ON ER."CountryId" = C."Id"
                WHERE (searchText IS NULL
                    OR ER."Title" ILIKE searchText
                    OR ER."EnglishTitle" ILIKE searchText
                    OR ER."Id"::TEXT ILIKE searchText)
                  AND (electionRoundStatus IS NULL OR ER."Status" = electionRoundStatus)
                  AND (countryId IS NULL OR ER."CountryId" = countryId)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The activated NGOs that don't monitor the election round yet, matching the search text (an ILIKE pattern on name
                -- or id; null doesn't filter).
                CREATE OR REPLACE FUNCTION "GetAvailableNgosForElectionRound"(
                    electionRoundId UUID,
                    searchText TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"   UUID,
                                "Name" TEXT
                            )
                AS
                $$
                SELECT N."Id",
                       N."Name"::text
                FROM "Ngos" N
                WHERE N."Status" = 'Activated'
                  AND N."Id" NOT IN (SELECT MN."NgoId"
                                     FROM "MonitoringNgos" MN
                                     WHERE MN."ElectionRoundId" = electionRoundId)
                  AND (searchText IS NULL
                    OR N."Name" ILIKE searchText
                    OR N."Id"::TEXT ILIKE searchText)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The polling stations an observer visited in an election round (from polling station information and form
                -- submissions), one row per station and monitoring observer with the latest visit time.
                CREATE OR REPLACE FUNCTION "GetObserverPollingStationVisits"(
                    electionRoundId UUID,
                    observerId UUID
                )
                    RETURNS TABLE
                            (
                                "PollingStationId"     UUID,
                                "Level1"               TEXT,
                                "Level2"               TEXT,
                                "Level3"               TEXT,
                                "Level4"               TEXT,
                                "Level5"               TEXT,
                                "Address"              TEXT,
                                "Number"               TEXT,
                                "Latitude"             DOUBLE PRECISION,
                                "Longitude"            DOUBLE PRECISION,
                                "MonitoringObserverId" UUID,
                                "VisitedAt"            TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT T."PollingStationId",
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Address"::text,
                       PS."Number"::text,
                       PS."Latitude",
                       PS."Longitude",
                       T."MonitoringObserverId",
                       MAX(T."LatestTimestamp")
                FROM (SELECT PSI."ElectionRoundId",
                             PSI."PollingStationId",
                             PSI."MonitoringObserverId",
                             PSI."LastUpdatedAt" "LatestTimestamp"
                      FROM "PollingStationInformation" PSI
                      WHERE PSI."ElectionRoundId" = electionRoundId
                      UNION
                      SELECT FS."ElectionRoundId",
                             FS."PollingStationId",
                             FS."MonitoringObserverId",
                             FS."LastUpdatedAt" "LatestTimestamp"
                      FROM "FormSubmissions" FS
                      WHERE FS."ElectionRoundId" = electionRoundId) T
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = T."MonitoringObserverId"
                    INNER JOIN "PollingStations" PS ON PS."Id" = T."PollingStationId"
                WHERE T."ElectionRoundId" = electionRoundId
                  AND MO."ObserverId" = observerId
                GROUP BY T."ElectionRoundId",
                         T."PollingStationId",
                         T."MonitoringObserverId",
                         PS."Id"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The records platform admins can move to another polling station: every form submission and incident report at a
                -- polling station and every quick report from a visited polling station (never citizen reports), with their observer,
                -- NGO, polling station and correction history (jsonb array, oldest first). Null arguments don't filter; searchText and
                -- pollingStationNumber are ILIKE patterns; correctedOnly keeps only records that were (true) or weren't (false) corrected.
                CREATE OR REPLACE FUNCTION "GetCorrectableRecords"(
                    electionRoundId UUID,
                    recordType TEXT,
                    recordId UUID,
                    monitoringNgoId UUID,
                    searchText TEXT,
                    pollingStationNumber TEXT,
                    correctedOnly BOOLEAN,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"               UUID,
                                "RecordType"       TEXT,
                                "ObserverName"     TEXT,
                                "NgoName"          TEXT,
                                "FormCode"         TEXT,
                                "FormName"         TEXT,
                                "SubmittedAt"      TIMESTAMP WITH TIME ZONE,
                                "PollingStationId" UUID,
                                "Number"           TEXT,
                                "Level1"           TEXT,
                                "Level2"           TEXT,
                                "Level3"           TEXT,
                                "Level4"           TEXT,
                                "Level5"           TEXT,
                                "Address"          TEXT,
                                "Corrections"      JSONB
                            )
                AS
                $$
                SELECT R."Id",
                       R."RecordType",
                       U."DisplayName",
                       N."Name"::text,
                       R."FormCode"::text,
                       R."FormName",
                       R."SubmittedAt",
                       PS."Id",
                       PS."Number"::text,
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Address"::text,
                       COALESCE((SELECT jsonb_agg(jsonb_build_object(
                                     'CorrectedAt', to_char(PSC."CorrectedAt" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS.US"Z"'),
                                     'From', jsonb_build_object('Id', FPS."Id", 'Number', FPS."Number", 'Level1', FPS."Level1", 'Level2', FPS."Level2", 'Level3', FPS."Level3", 'Level4', FPS."Level4", 'Level5', FPS."Level5", 'Address', FPS."Address"),
                                     'To', jsonb_build_object('Id', TPS."Id", 'Number', TPS."Number", 'Level1', TPS."Level1", 'Level2', TPS."Level2", 'Level3', TPS."Level3", 'Level4', TPS."Level4", 'Level5', TPS."Level5", 'Address', TPS."Address")
                                   ) ORDER BY PSC."CorrectedAt")
                                 FROM "PollingStationCorrections" PSC
                                     INNER JOIN "PollingStations" FPS ON FPS."Id" = PSC."FromPollingStationId"
                                     INNER JOIN "PollingStations" TPS ON TPS."Id" = PSC."ToPollingStationId"
                                 WHERE PSC."RecordType" = R."RecordType" AND PSC."RecordId" = R."Id"), '[]'::jsonb)
                FROM (SELECT 'FormSubmission' AS "RecordType",
                             FS."Id",
                             FS."MonitoringObserverId",
                             FS."PollingStationId",
                             F."Code" AS "FormCode",
                             F."Name" ->> F."DefaultLanguage" AS "FormName",
                             FS."LastUpdatedAt" AS "SubmittedAt"
                      FROM "FormSubmissions" FS
                          INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                      WHERE FS."ElectionRoundId" = electionRoundId
                        AND (recordType IS NULL OR recordType = 'FormSubmission')
                        AND (recordId IS NULL OR FS."Id" = recordId)
                      UNION ALL
                      SELECT 'QuickReport' AS "RecordType",
                             QR."Id",
                             QR."MonitoringObserverId",
                             QR."PollingStationId",
                             NULL AS "FormCode",
                             NULL AS "FormName",
                             QR."LastUpdatedAt" AS "SubmittedAt"
                      FROM "QuickReports" QR
                      WHERE QR."ElectionRoundId" = electionRoundId
                        AND QR."QuickReportLocationType" = 'VisitedPollingStation'
                        AND QR."PollingStationId" IS NOT NULL
                        AND (recordType IS NULL OR recordType = 'QuickReport')
                        AND (recordId IS NULL OR QR."Id" = recordId)
                      UNION ALL
                      SELECT 'IncidentReport' AS "RecordType",
                             IR."Id",
                             IR."MonitoringObserverId",
                             IR."PollingStationId",
                             F."Code" AS "FormCode",
                             F."Name" ->> F."DefaultLanguage" AS "FormName",
                             IR."LastUpdatedAt" AS "SubmittedAt"
                      FROM "IncidentReports" IR
                          INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                      WHERE IR."ElectionRoundId" = electionRoundId
                        AND IR."PollingStationId" IS NOT NULL
                        AND (recordType IS NULL OR recordType = 'IncidentReport')
                        AND (recordId IS NULL OR IR."Id" = recordId)) R
                    INNER JOIN "PollingStations" PS ON PS."Id" = R."PollingStationId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = R."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                  AND (searchText IS NULL OR U."DisplayName" ILIKE searchText OR N."Name" ILIKE searchText)
                  AND (pollingStationNumber IS NULL OR PS."Number" ILIKE pollingStationNumber)
                  AND (correctedOnly IS NULL OR correctedOnly = EXISTS (SELECT 1
                                                                        FROM "PollingStationCorrections" PSC
                                                                        WHERE PSC."ElectionRoundId" = electionRoundId
                                                                          AND PSC."RecordType" = R."RecordType"
                                                                          AND PSC."RecordId" = R."Id"))
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The polling station information submitted in an election round, with its polling station, NGO, observer and minutes
                -- monitoring. Null arguments don't filter; fromDate/toDate are timestamps as text (compared with LastUpdatedAt, both
                -- inclusive); searchText is an ILIKE pattern on the polling station number or address.
                CREATE OR REPLACE FUNCTION "GetPollingStationInformationData"(
                    electionRoundId UUID,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    monitoringNgoId UUID,
                    isCompleted BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    searchText TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                        UUID,
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "Number"                    TEXT,
                                "Address"                   TEXT,
                                "MonitoringNgoId"           UUID,
                                "NgoName"                   TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "ObserverEmail"             TEXT,
                                "ObserverPhoneNumber"       TEXT,
                                "ArrivalTime"               TIMESTAMP WITH TIME ZONE,
                                "DepartureTime"             TIMESTAMP WITH TIME ZONE,
                                "MinutesMonitoring"         DOUBLE PRECISION,
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "IsCompleted"               BOOLEAN,
                                "LastUpdatedAt"             TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT PSI."Id",
                       PS."Id",
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Number"::text,
                       PS."Address"::text,
                       MN."Id",
                       N."Name"::text,
                       MO."Id",
                       U."DisplayName",
                       U."Email"::text,
                       U."PhoneNumber",
                       PSI."ArrivalTime",
                       PSI."DepartureTime",
                       COALESCE("ComputeMinutesMonitoring"(PSI."ArrivalTime", PSI."DepartureTime", PSI."Breaks"), 0),
                       PSI."NumberOfQuestionsAnswered",
                       PSI."NumberOfFlaggedAnswers",
                       PSI."IsCompleted",
                       PSI."LastUpdatedAt"
                FROM "PollingStationInformation" PSI
                    INNER JOIN "PollingStations" PS ON PS."Id" = PSI."PollingStationId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = PSI."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE PSI."ElectionRoundId" = electionRoundId
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                  AND (isCompleted IS NULL OR PSI."IsCompleted" = isCompleted)
                  AND (fromDate IS NULL OR PSI."LastUpdatedAt" >= fromDate::timestamp)
                  AND (toDate IS NULL OR PSI."LastUpdatedAt" <= toDate::timestamp)
                  AND (searchText IS NULL OR PS."Number" ILIKE searchText OR PS."Address" ILIKE searchText)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Whether the observer has a form submission with at least one answer at the polling station in the election round.
                CREATE OR REPLACE FUNCTION "ObserverHasFormAnswersAtPollingStation"(
                    electionRoundId UUID,
                    pollingStationId UUID,
                    observerId UUID
                )
                    RETURNS BOOLEAN
                AS
                $$
                SELECT EXISTS (SELECT 1
                               FROM "FormSubmissions" FS
                                   INNER JOIN "MonitoringObservers" MO ON FS."MonitoringObserverId" = MO."Id"
                               WHERE FS."ElectionRoundId" = electionRoundId
                                 AND MO."ObserverId" = observerId
                                 AND FS."PollingStationId" = pollingStationId
                                 AND JSONB_ARRAY_LENGTH(FS."Answers") > 0)
                $$ LANGUAGE sql STABLE;
                """);

            // form-submissions
            migrationBuilder.Sql(
                """
                -- The first and last submission timestamps (form submissions and polling station information) the NGO can see in a
                -- round for the data source. Always one row (nulls when there are no submissions).
                CREATE OR REPLACE FUNCTION "GetFormSubmissionFilterTimestamps"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "FirstSubmissionTimestamp" TIMESTAMP WITH TIME ZONE,
                                "LastSubmissionTimestamp"  TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                WITH "CombinedTimestamps" AS (
                    SELECT MIN("LastUpdatedAt") AS "FirstSubmissionTimestamp",
                           MAX("LastUpdatedAt") AS "LastSubmissionTimestamp"
                    FROM "FormSubmissions" FS
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                        INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF ON FS."FormId" = AF."FormId"
                    WHERE FS."ElectionRoundId" = electionRoundId
                    UNION ALL
                    SELECT MIN("LastUpdatedAt") AS "FirstSubmissionTimestamp",
                           MAX("LastUpdatedAt") AS "LastSubmissionTimestamp"
                    FROM "PollingStationInformation" PSI
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                    WHERE PSI."ElectionRoundId" = electionRoundId
                )
                SELECT MIN("FirstSubmissionTimestamp") AS "FirstSubmissionTimestamp",
                       MAX("LastSubmissionTimestamp")  AS "LastSubmissionTimestamp"
                FROM "CombinedTimestamps"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The forms (including the polling station information form) that have submissions the NGO can see in a round for the
                -- data source, one row per form and source.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionFilterForms"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "FormId"          UUID,
                                "FormCode"        TEXT,
                                "FormName"        JSONB,
                                "DefaultLanguage" TEXT,
                                "Languages"       TEXT[],
                                "FormStatus"      TEXT
                            )
                AS
                $$
                SELECT DISTINCT F."Id" AS "FormId",
                                F."Code"::text AS "FormCode",
                                F."Name" AS "FormName",
                                F."DefaultLanguage"::text AS "DefaultLanguage",
                                F."Languages",
                                F."Status" AS "FormStatus"
                FROM "FormSubmissions" FS
                    INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                    INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF ON FS."FormId" = AF."FormId"
                WHERE FS."ElectionRoundId" = electionRoundId
                UNION ALL
                SELECT DISTINCT F."Id" AS "FormId",
                                F."Code"::text AS "FormCode",
                                F."Name" AS "FormName",
                                F."DefaultLanguage"::text AS "DefaultLanguage",
                                F."Languages",
                                F."Status" AS "FormStatus"
                FROM "PollingStationInformation" PSI
                    INNER JOIN "PollingStationInformationForms" F ON F."Id" = PSI."PollingStationInformationFormId"
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                    INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF ON F."Id" = AF."FormId"
                WHERE PSI."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The observers with submissions (form submissions or polling station information) the NGO can see in a round for the
                -- data source.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionFilterObservers"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "MonitoringObserverId" UUID,
                                "DisplayName"          TEXT,
                                "Email"                TEXT,
                                "AccountStatus"        TEXT
                            )
                AS
                $$
                SELECT DISTINCT MO."MonitoringObserverId",
                                MO."DisplayName",
                                MO."Email",
                                MO."AccountStatus"
                FROM "FormSubmissions" FS
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
                    INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF ON FS."FormId" = AF."FormId"
                WHERE FS."ElectionRoundId" = electionRoundId
                UNION
                SELECT DISTINCT MO."MonitoringObserverId",
                                MO."DisplayName",
                                MO."Email",
                                MO."AccountStatus"
                FROM "PollingStationInformation" PSI
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                WHERE PSI."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The NGO's published, non citizen-reporting forms in a round for the data source with the number of submissions,
                -- flagged answers, media files and notes of the submissions (including polling station information) matching the
                -- filters. Null filters (and an empty tags filter) match everything.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionsByForm"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    coalitionMemberId UUID,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    hasFlaggedAnswers BOOLEAN,
                    followUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    hasNotes BOOLEAN,
                    hasAttachments BOOLEAN,
                    questionsAnswered TEXT
                )
                    RETURNS TABLE
                            (
                                "FormId"                 UUID,
                                "FormCode"               TEXT,
                                "FormType"               TEXT,
                                "FormStatus"             TEXT,
                                "FormName"               JSONB,
                                "DefaultLanguage"        TEXT,
                                "NumberOfSubmissions"    BIGINT,
                                "NumberOfFlaggedAnswers" BIGINT,
                                "NumberOfMediaFiles"     BIGINT,
                                "NumberOfNotes"          BIGINT
                            )
                AS
                $$
                WITH
                    FORM_SUBMISSIONS AS (
                        SELECT
                            FS."Id" AS "SubmissionId",
                            FS."NumberOfFlaggedAnswers",
                            FS."NumberOfQuestionsAnswered",
                            FS."Answers",
                            FS."FollowUpStatus",
                            FS."IsCompleted",
                            "LastUpdatedAt" AS "TimeSubmitted",
                            COALESCE(
                                (
                                    SELECT
                                        JSONB_AGG(
                                            JSONB_BUILD_OBJECT(
                                                'QuestionId',
                                                "QuestionId",
                                                'FileName',
                                                "FileName",
                                                'MimeType',
                                                "MimeType",
                                                'FilePath',
                                                "FilePath",
                                                'UploadedFileName',
                                                "UploadedFileName",
                                                'TimeSubmitted',
                                                "LastUpdatedAt"
                                            )
                                        )
                                    FROM
                                        "Attachments" A
                                    WHERE
                                        (
                                            (A."FormId" = FS."FormId" AND FS."PollingStationId" = A."PollingStationId") -- backwards compatibility
                                            OR A."SubmissionId" = FS."Id"
                                        )
                                        AND A."MonitoringObserverId" = FS."MonitoringObserverId"
                                        AND A."IsDeleted" = FALSE
                                        AND A."IsCompleted" = TRUE
                                ),
                                '[]'::JSONB
                            ) AS "Attachments",
                            COALESCE(
                                (
                                    SELECT
                                        JSONB_AGG(
                                            JSONB_BUILD_OBJECT(
                                                'QuestionId',
                                                "QuestionId",
                                                'Text',
                                                "Text",
                                                'TimeSubmitted',
                                                "LastUpdatedAt"
                                            )
                                        )
                                    FROM
                                        "Notes" N
                                    WHERE
                                        (
                                            (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId") -- backwards compatibility
                                            OR N."SubmissionId" = FS."Id"
                                        )
                                        AND N."MonitoringObserverId" = FS."MonitoringObserverId"
                                ),
                                '[]'::JSONB
                            ) AS "Notes",
                            PS."Id" AS "PollingStationId",
                            PS."Level1" AS "PollingStationLevel1",
                            PS."Level2" AS "PollingStationLevel2",
                            PS."Level3" AS "PollingStationLevel3",
                            PS."Level4" AS "PollingStationLevel4",
                            PS."Level5" AS "PollingStationLevel5",
                            PS."Number" AS "PollingStationNumber",
                            F."Id" AS "FormId",
                            F."Code" AS "FormCode",
                            F."FormType" AS "FormType",
                            F."Name" AS "FormName",
                            F."DefaultLanguage" AS "FormDefaultLanguage",
                            F."Questions" AS "FormQuestions",
                            MO."MonitoringObserverId",
                            MO."DisplayName",
                            MO."Tags",
                            MO."Status",
                            MO."Email",
                            MO."PhoneNumber",
                            MO."NgoId"
                        FROM
                            "FormSubmissions" FS
                            INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON FS."MonitoringObserverId" = MO."MonitoringObserverId"
                            INNER JOIN "PollingStations" PS ON PS."Id" = FS."PollingStationId"
                            INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                        WHERE
                            FS."ElectionRoundId" = electionRoundId
                    ),
                    PSI_SUBMISSIONS AS (
                        SELECT
                            PSI."Id" AS "SubmissionId",
                            PSI."NumberOfFlaggedAnswers",
                            PSI."NumberOfQuestionsAnswered",
                            PSI."Answers",
                            PSI."FollowUpStatus",
                            PSI."IsCompleted",
                            "LastUpdatedAt" AS "TimeSubmitted",
                            '[]'::JSONB AS "Attachments",
                            '[]'::JSONB AS "Notes",
                            PS."Id" AS "PollingStationId",
                            PS."Level1" AS "PollingStationLevel1",
                            PS."Level2" AS "PollingStationLevel2",
                            PS."Level3" AS "PollingStationLevel3",
                            PS."Level4" AS "PollingStationLevel4",
                            PS."Level5" AS "PollingStationLevel5",
                            PS."Number" AS "PollingStationNumber",
                            PSIF."Id" AS "FormId",
                            PSIF."Code" AS "FormCode",
                            PSIF."FormType" AS "FormType",
                            PSIF."Name" AS "FormName",
                            PSIF."DefaultLanguage" AS "FormDefaultLanguage",
                            PSIF."Questions" AS "FormQuestions",
                            MO."MonitoringObserverId",
                            MO."DisplayName",
                            MO."Tags",
                            MO."Status",
                            MO."Email",
                            MO."PhoneNumber",
                            MO."NgoId"
                        FROM
                            "PollingStationInformation" PSI
                            INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON PSI."MonitoringObserverId" = MO."MonitoringObserverId"
                            INNER JOIN "PollingStations" PS ON PSI."PollingStationId" = PS."Id"
                            INNER JOIN "PollingStationInformationForms" PSIF ON PSIF."Id" = PSI."PollingStationInformationFormId"
                        WHERE
                            PSI."ElectionRoundId" = electionRoundId
                    ),
                    ALL_SUBMISSIONS AS (
                        SELECT * FROM PSI_SUBMISSIONS
                        UNION
                        SELECT * FROM FORM_SUBMISSIONS
                    ),
                    FILTERED_SUBMISSIONS AS (
                        SELECT
                            *
                        FROM
                            ALL_SUBMISSIONS FS
                        WHERE
                            (coalitionMemberId IS NULL OR FS."NgoId" = coalitionMemberId)
                            AND (level1 IS NULL OR FS."PollingStationLevel1" = level1)
                            AND (level2 IS NULL OR FS."PollingStationLevel2" = level2)
                            AND (level3 IS NULL OR FS."PollingStationLevel3" = level3)
                            AND (level4 IS NULL OR FS."PollingStationLevel4" = level4)
                            AND (level5 IS NULL OR FS."PollingStationLevel5" = level5)
                            AND (pollingStationNumber IS NULL OR FS."PollingStationNumber" = pollingStationNumber)
                            AND (
                                hasFlaggedAnswers IS NULL
                                OR ("NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                                OR ("NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE)
                            )
                            AND (followUpStatus IS NULL OR "FollowUpStatus" = followUpStatus)
                            AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR FS."Tags" && tagsFilter)
                            AND (monitoringObserverStatus IS NULL OR FS."Status" = monitoringObserverStatus)
                            AND (formId IS NULL OR FS."FormId" = formId)
                            AND (
                                questionsAnswered IS NULL
                                OR (questionsAnswered = 'All' AND JSONB_ARRAY_LENGTH(FS."FormQuestions") = FS."NumberOfQuestionsAnswered")
                                OR (questionsAnswered = 'Some' AND JSONB_ARRAY_LENGTH(FS."FormQuestions") <> FS."NumberOfQuestionsAnswered")
                                OR (questionsAnswered = 'None' AND FS."NumberOfQuestionsAnswered" = 0)
                            )
                            AND (
                                hasAttachments IS NULL
                                OR (JSONB_ARRAY_LENGTH(FS."Attachments") = 0 AND hasAttachments = FALSE)
                                OR (JSONB_ARRAY_LENGTH(FS."Attachments") > 0 AND hasAttachments = TRUE)
                            )
                            AND (
                                hasNotes IS NULL
                                OR (JSONB_ARRAY_LENGTH(FS."Notes") = 0 AND hasNotes = FALSE)
                                OR (JSONB_ARRAY_LENGTH(FS."Notes") > 0 AND hasNotes = TRUE)
                            )
                    )
                SELECT
                    AF."FormId",
                    AF."FormCode"::text,
                    AF."FormType",
                    AF."FormStatus",
                    AF."FormName",
                    AF."FormDefaultLanguage"::text AS "DefaultLanguage",
                    COUNT(FS."SubmissionId") AS "NumberOfSubmissions",
                    COALESCE(SUM(FS."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                    COALESCE(SUM(JSONB_ARRAY_LENGTH(FS."Attachments")), 0) AS "NumberOfMediaFiles",
                    COALESCE(SUM(JSONB_ARRAY_LENGTH(FS."Notes")), 0) AS "NumberOfNotes"
                FROM
                    "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF
                    LEFT JOIN FILTERED_SUBMISSIONS FS ON FS."FormId" = AF."FormId"
                WHERE
                    AF."FormStatus" <> 'Drafted'
                    AND AF."FormType" NOT IN ('CitizenReporting')
                GROUP BY
                    AF."FormId",
                    AF."FormCode",
                    AF."FormType",
                    AF."FormStatus",
                    AF."FormName",
                    AF."FormDefaultLanguage"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The observers available to the NGO in a round for the data source with their number of flagged answers, locations
                -- and submissions (including polling station information) and whether any of their submissions needs follow-up.
                -- Null filters (an empty search text, an empty tags filter) match everything; any non-null needsFollowUp keeps only the
                -- observers needing follow-up.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionObserverOverviews"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    coalitionMemberId UUID,
                    searchText TEXT,
                    tagsFilter TEXT[],
                    needsFollowUp TEXT
                )
                    RETURNS TABLE
                            (
                                "MonitoringObserverId"   UUID,
                                "ObserverName"           TEXT,
                                "PhoneNumber"            TEXT,
                                "Email"                  TEXT,
                                "Tags"                   TEXT[],
                                "NgoName"                TEXT,
                                "NumberOfFlaggedAnswers" BIGINT,
                                "NumberOfLocations"      BIGINT,
                                "NumberOfFormsSubmitted" BIGINT,
                                "FollowUpStatus"         TEXT,
                                "IsOwnObserver"          BOOLEAN
                            )
                AS
                $$
                SELECT
                    "MonitoringObserverId",
                    "ObserverName",
                    "PhoneNumber",
                    "Email",
                    "Tags",
                    "NgoName",
                    "NumberOfFlaggedAnswers",
                    "NumberOfLocations",
                    "NumberOfFormsSubmitted",
                    "FollowUpStatus",
                    "IsOwnObserver"
                FROM
                    (
                        SELECT
                            MO."MonitoringObserverId" "MonitoringObserverId",
                            MO."DisplayName" "ObserverName",
                            MO."PhoneNumber",
                            MO."Email",
                            MO."Tags",
                            MO."NgoName"::text AS "NgoName",
                            MO."IsOwnObserver",
                            COALESCE(
                                (
                                    SELECT SUM("NumberOfFlaggedAnswers")
                                    FROM "FormSubmissions" FS
                                        INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) F ON F."FormId" = FS."FormId"
                                    WHERE FS."MonitoringObserverId" = MO."MonitoringObserverId"
                                ),
                                0
                            ) AS "NumberOfFlaggedAnswers",
                            (
                                SELECT COUNT(*)
                                FROM
                                    (
                                        SELECT PSI."PollingStationId"
                                        FROM "PollingStationInformation" PSI
                                        WHERE PSI."MonitoringObserverId" = MO."MonitoringObserverId"
                                          AND PSI."ElectionRoundId" = electionRoundId
                                        UNION
                                        SELECT FS."PollingStationId"
                                        FROM "FormSubmissions" FS
                                            INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) F ON F."FormId" = FS."FormId"
                                        WHERE FS."MonitoringObserverId" = MO."MonitoringObserverId"
                                          AND FS."ElectionRoundId" = electionRoundId
                                    ) TMP
                            ) AS "NumberOfLocations",
                            (
                                SELECT COUNT(*)
                                FROM
                                    (
                                        SELECT PSI."Id"
                                        FROM "PollingStationInformation" PSI
                                        WHERE PSI."MonitoringObserverId" = MO."MonitoringObserverId"
                                          AND PSI."ElectionRoundId" = electionRoundId
                                        UNION
                                        SELECT FS."Id"
                                        FROM "FormSubmissions" FS
                                            INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) F ON F."FormId" = FS."FormId"
                                        WHERE FS."MonitoringObserverId" = MO."MonitoringObserverId"
                                          AND FS."ElectionRoundId" = electionRoundId
                                    ) TMP
                            ) AS "NumberOfFormsSubmitted",
                            (
                                CASE WHEN EXISTS (
                                    SELECT 1
                                    FROM "FormSubmissions" FS
                                        INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) F ON F."FormId" = FS."FormId"
                                    WHERE FS."FollowUpStatus" = 'NeedsFollowUp'
                                      AND FS."MonitoringObserverId" = MO."MonitoringObserverId"
                                      AND FS."ElectionRoundId" = electionRoundId
                                ) THEN 'NeedsFollowUp' ELSE NULL END
                            ) AS "FollowUpStatus"
                        FROM
                            "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO
                        WHERE
                            (coalitionMemberId IS NULL OR MO."NgoId" = coalitionMemberId)
                            AND (
                                searchText IS NULL
                                OR searchText = ''
                                OR MO."MonitoringObserverId"::TEXT ILIKE searchText
                                OR MO."DisplayName" ILIKE searchText
                                OR MO."Email" ILIKE searchText
                                OR MO."PhoneNumber" ILIKE searchText
                            )
                            AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR MO."Tags" && tagsFilter)
                    ) T
                WHERE
                    (needsFollowUp IS NULL OR T."FollowUpStatus" = 'NeedsFollowUp')
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- A form submission or polling station information visible to the NGO (Coalition data source) in a round, with its
                -- form, polling station, observer, answers, notes and attachments. No row when not found.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionView"(
                    electionRoundId UUID,
                    ngoId UUID,
                    submissionId UUID
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"         UUID,
                                "FormId"               UUID,
                                "TimeSubmitted"        TIMESTAMP WITH TIME ZONE,
                                "FormCode"             TEXT,
                                "FormName"             JSONB,
                                "FormType"             TEXT,
                                "PollingStationId"     UUID,
                                "Level1"               TEXT,
                                "Level2"               TEXT,
                                "Level3"               TEXT,
                                "Level4"               TEXT,
                                "Level5"               TEXT,
                                "Number"               TEXT,
                                "MonitoringObserverId" UUID,
                                "ObserverName"         TEXT,
                                "Email"                TEXT,
                                "PhoneNumber"          TEXT,
                                "Tags"                 TEXT[],
                                "NgoName"              TEXT,
                                "IsOwnObserver"        BOOLEAN,
                                "Attachments"          JSONB,
                                "Notes"                JSONB,
                                "Answers"              JSONB,
                                "Questions"            JSONB,
                                "DefaultLanguage"      TEXT,
                                "Languages"            TEXT[],
                                "FollowUpStatus"       TEXT,
                                "ArrivalTime"          TIMESTAMP WITH TIME ZONE,
                                "DepartureTime"        TIMESTAMP WITH TIME ZONE,
                                "Breaks"               JSONB,
                                "IsCompleted"          BOOLEAN
                            )
                AS
                $$
                WITH submissions AS
                         (SELECT psi."Id" AS "SubmissionId",
                                 'PSI' AS "FormType",
                                 'PSI' AS "FormCode",
                                 psif."Name" AS "FormName",
                                 psi."PollingStationId",
                                 psi."MonitoringObserverId",
                                 psi."Answers",
                                 psif."Questions" AS "Questions",
                                 psif."DefaultLanguage" AS "DefaultLanguage",
                                 psif."Languages" AS "Languages",
                                 psif."Id" AS "FormId",
                                 psi."FollowUpStatus" AS "FollowUpStatus",
                                 '[]'::jsonb AS "Attachments",
                                 '[]'::jsonb AS "Notes",
                                 "LastUpdatedAt" AS "TimeSubmitted",
                                 psi."ArrivalTime",
                                 psi."DepartureTime",
                                 psi."Breaks",
                                 psi."IsCompleted"
                          FROM "PollingStationInformation" psi
                              INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = psi."MonitoringObserverId"
                              INNER JOIN "PollingStationInformationForms" psif ON psif."ElectionRoundId" = psi."ElectionRoundId"
                          WHERE psi."Id" = submissionId AND psi."ElectionRoundId" = electionRoundId
                          UNION ALL
                          SELECT fs."Id" AS "SubmissionId",
                                 f."FormType" AS "FormType",
                                 f."Code" AS "FormCode",
                                 f."Name" AS "FormName",
                                 fs."PollingStationId",
                                 fs."MonitoringObserverId",
                                 fs."Answers",
                                 f."Questions",
                                 f."DefaultLanguage",
                                 f."Languages",
                                 f."Id" AS "FormId",
                                 fs."FollowUpStatus",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'FileName', "FileName", 'MimeType', "MimeType", 'FilePath', "FilePath", 'UploadedFileName', "UploadedFileName", 'TimeSubmitted', "LastUpdatedAt"))
                                           FROM "Attachments" a
                                           WHERE ((a."FormId" = fs."FormId" AND fs."PollingStationId" = a."PollingStationId") -- backwards compatibility
                                                  OR a."SubmissionId" = fs."Id")
                                             AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                                             AND a."IsDeleted" = false
                                             AND a."IsCompleted" = true)) AS "Attachments",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'Text', "Text", 'TimeSubmitted', "LastUpdatedAt"))
                                           FROM "Notes" n
                                           WHERE ((n."FormId" = fs."FormId" AND fs."PollingStationId" = n."PollingStationId") -- backwards compatibility
                                                  OR n."SubmissionId" = fs."Id")
                                             AND n."MonitoringObserverId" = fs."MonitoringObserverId"), '[]'::JSONB) AS "Notes",
                                 "LastUpdatedAt" AS "TimeSubmitted",
                                 NULL AS "ArrivalTime",
                                 NULL AS "DepartureTime",
                                 '[]'::jsonb AS "Breaks",
                                 fs."IsCompleted"
                          FROM "FormSubmissions" fs
                              INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = fs."MonitoringObserverId"
                              INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                          WHERE fs."Id" = submissionId AND fs."ElectionRoundId" = electionRoundId)
                SELECT s."SubmissionId",
                       s."FormId",
                       s."TimeSubmitted",
                       s."FormCode"::text,
                       s."FormName",
                       s."FormType",
                       ps."Id" AS "PollingStationId",
                       ps."Level1"::text,
                       ps."Level2"::text,
                       ps."Level3"::text,
                       ps."Level4"::text,
                       ps."Level5"::text,
                       ps."Number"::text,
                       s."MonitoringObserverId",
                       AMO."DisplayName" AS "ObserverName",
                       AMO."Email",
                       AMO."PhoneNumber",
                       AMO."Tags",
                       AMO."NgoName"::text,
                       AMO."IsOwnObserver",
                       s."Attachments",
                       s."Notes",
                       s."Answers",
                       s."Questions",
                       s."DefaultLanguage"::text,
                       s."Languages",
                       s."FollowUpStatus",
                       s."ArrivalTime",
                       s."DepartureTime",
                       s."Breaks",
                       s."IsCompleted"
                FROM submissions s
                    INNER JOIN "PollingStations" ps ON ps."Id" = s."PollingStationId"
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = s."MonitoringObserverId"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- A form submission or polling station information visible to the NGO (Coalition data source) in a round, with its
                -- form, polling station, observer, answers, notes, attachments and polling station corrections (oldest first). No row
                -- when not found.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionViewV2"(
                    electionRoundId UUID,
                    ngoId UUID,
                    submissionId UUID
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"              UUID,
                                "FormId"                    UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormCode"                  TEXT,
                                "FormName"                  JSONB,
                                "FormType"                  TEXT,
                                "DefaultLanguage"           TEXT,
                                "Languages"                 TEXT[],
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "Number"                    TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "Email"                     TEXT,
                                "PhoneNumber"               TEXT,
                                "Tags"                      TEXT[],
                                "NgoName"                   TEXT,
                                "IsOwnObserver"             BOOLEAN,
                                "Attachments"               JSONB,
                                "Notes"                     JSONB,
                                "Answers"                   JSONB,
                                "Questions"                 JSONB,
                                "FollowUpStatus"            TEXT,
                                "ArrivalTime"               TIMESTAMP WITH TIME ZONE,
                                "DepartureTime"             TIMESTAMP WITH TIME ZONE,
                                "Breaks"                    JSONB,
                                "IsCompleted"               BOOLEAN,
                                "PollingStationCorrections" JSONB
                            )
                AS
                $$
                WITH submissions AS
                         (SELECT psi."Id" AS "SubmissionId",
                                 'PSI' AS "FormType",
                                 'PSI' AS "FormCode",
                                 psif."Name" AS "FormName",
                                 psi."PollingStationId",
                                 psi."MonitoringObserverId",
                                 psi."Answers",
                                 psif."Questions" AS "Questions",
                                 psif."DefaultLanguage" AS "DefaultLanguage",
                                 psif."Languages" AS "Languages",
                                 psif."Id" AS "FormId",
                                 psi."FollowUpStatus" AS "FollowUpStatus",
                                 '[]'::jsonb AS "Attachments",
                                 '[]'::jsonb AS "Notes",
                                 "LastUpdatedAt" AS "TimeSubmitted",
                                 psi."ArrivalTime",
                                 psi."DepartureTime",
                                 psi."Breaks",
                                 psi."IsCompleted",
                                 '[]'::jsonb AS "PollingStationCorrections"
                          FROM "PollingStationInformation" psi
                              INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = psi."MonitoringObserverId"
                              INNER JOIN "PollingStationInformationForms" psif ON psi."ElectionRoundId" = psif."ElectionRoundId"
                          WHERE psi."Id" = submissionId AND psi."ElectionRoundId" = electionRoundId
                          UNION ALL
                          SELECT fs."Id" AS "SubmissionId",
                                 f."FormType" AS "FormType",
                                 f."Code" AS "FormCode",
                                 f."Name" AS "FormName",
                                 fs."PollingStationId",
                                 fs."MonitoringObserverId",
                                 fs."Answers",
                                 f."Questions",
                                 f."DefaultLanguage",
                                 f."Languages",
                                 f."Id" AS "FormId",
                                 fs."FollowUpStatus",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'FileName', "FileName", 'MimeType', "MimeType", 'FilePath', "FilePath", 'UploadedFileName', "UploadedFileName", 'TimeSubmitted', "LastUpdatedAt"))
                                           FROM "Attachments" a
                                           WHERE ((a."FormId" = fs."FormId" AND fs."PollingStationId" = a."PollingStationId") -- backwards compatibility
                                                  OR a."SubmissionId" = fs."Id")
                                             AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                                             AND a."IsDeleted" = false
                                             AND a."IsCompleted" = true), '[]'::JSONB) AS "Attachments",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'Text', "Text", 'TimeSubmitted', "LastUpdatedAt"))
                                           FROM "Notes" n
                                           WHERE ((n."FormId" = fs."FormId" AND fs."PollingStationId" = n."PollingStationId") -- backwards compatibility
                                                  OR n."SubmissionId" = fs."Id")
                                             AND n."MonitoringObserverId" = fs."MonitoringObserverId"), '[]'::JSONB) AS "Notes",
                                 "LastUpdatedAt" AS "TimeSubmitted",
                                 NULL AS "ArrivalTime",
                                 NULL AS "DepartureTime",
                                 '[]'::jsonb AS "Breaks",
                                 fs."IsCompleted",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object(
                                                    'CorrectedAt', to_char(PSC."CorrectedAt" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS.US"Z"'),
                                                    'From', jsonb_build_object('Id', FPS."Id", 'Number', FPS."Number", 'Level1', FPS."Level1", 'Level2', FPS."Level2", 'Level3', FPS."Level3", 'Level4', FPS."Level4", 'Level5', FPS."Level5", 'Address', FPS."Address"),
                                                    'To', jsonb_build_object('Id', TPS."Id", 'Number', TPS."Number", 'Level1', TPS."Level1", 'Level2', TPS."Level2", 'Level3', TPS."Level3", 'Level4', TPS."Level4", 'Level5', TPS."Level5", 'Address', TPS."Address")
                                                  ) ORDER BY PSC."CorrectedAt")
                                           FROM "PollingStationCorrections" PSC
                                               INNER JOIN "PollingStations" FPS ON FPS."Id" = PSC."FromPollingStationId"
                                               INNER JOIN "PollingStations" TPS ON TPS."Id" = PSC."ToPollingStationId"
                                           WHERE PSC."RecordType" = 'FormSubmission' AND PSC."RecordId" = fs."Id"), '[]'::jsonb) AS "PollingStationCorrections"
                          FROM "FormSubmissions" fs
                              INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = fs."MonitoringObserverId"
                              INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                          WHERE fs."Id" = submissionId AND fs."ElectionRoundId" = electionRoundId)
                SELECT s."SubmissionId",
                       s."FormId",
                       s."TimeSubmitted",
                       s."FormCode"::text,
                       s."FormName",
                       s."FormType",
                       s."DefaultLanguage"::text,
                       s."Languages",
                       ps."Id" AS "PollingStationId",
                       ps."Level1"::text,
                       ps."Level2"::text,
                       ps."Level3"::text,
                       ps."Level4"::text,
                       ps."Level5"::text,
                       ps."Number"::text,
                       s."MonitoringObserverId",
                       AMO."DisplayName" AS "ObserverName",
                       AMO."Email",
                       AMO."PhoneNumber",
                       AMO."Tags",
                       AMO."NgoName"::text,
                       AMO."IsOwnObserver",
                       s."Attachments",
                       s."Notes",
                       s."Answers",
                       s."Questions",
                       s."FollowUpStatus",
                       s."ArrivalTime",
                       s."DepartureTime",
                       s."Breaks",
                       s."IsCompleted",
                       s."PollingStationCorrections"
                FROM submissions s
                    INNER JOIN "PollingStations" ps ON ps."Id" = s."PollingStationId"
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = s."MonitoringObserverId"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Every form submission in a round (any NGO) with its form, polling station, NGO and observer, matching the filters
                -- (platform admin listing). Null filters match everything; fromDate/toDate are ISO 8601 strings compared as timestamps.
                CREATE OR REPLACE FUNCTION "GetElectionRoundFormSubmissionRows"(
                    electionRoundId UUID,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    monitoringNgoId UUID,
                    formId UUID,
                    isCompleted BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    searchText TEXT
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"              UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormId"                    UUID,
                                "FormCode"                  TEXT,
                                "FormType"                  TEXT,
                                "FormName"                  JSONB,
                                "DefaultLanguage"           TEXT,
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "Number"                    TEXT,
                                "Address"                   TEXT,
                                "MonitoringNgoId"           UUID,
                                "NgoName"                   TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "ObserverEmail"             TEXT,
                                "ObserverPhoneNumber"       TEXT,
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "FollowUpStatus"            TEXT,
                                "IsCompleted"               BOOLEAN
                            )
                AS
                $$
                SELECT FS."Id" AS "SubmissionId",
                       FS."LastUpdatedAt" AS "TimeSubmitted",
                       F."Id" AS "FormId",
                       F."Code"::text AS "FormCode",
                       F."FormType",
                       F."Name" AS "FormName",
                       F."DefaultLanguage"::text AS "DefaultLanguage",
                       PS."Id" AS "PollingStationId",
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Number"::text,
                       PS."Address"::text,
                       MN."Id" AS "MonitoringNgoId",
                       N."Name"::text AS "NgoName",
                       MO."Id" AS "MonitoringObserverId",
                       U."DisplayName" AS "ObserverName",
                       U."Email"::text AS "ObserverEmail",
                       U."PhoneNumber" AS "ObserverPhoneNumber",
                       FS."NumberOfQuestionsAnswered",
                       FS."NumberOfFlaggedAnswers",
                       FS."FollowUpStatus",
                       FS."IsCompleted"
                FROM "FormSubmissions" FS
                    INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    INNER JOIN "PollingStations" PS ON PS."Id" = FS."PollingStationId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE FS."ElectionRoundId" = electionRoundId
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                  AND (formId IS NULL OR FS."FormId" = formId)
                  AND (isCompleted IS NULL OR FS."IsCompleted" = isCompleted)
                  AND (fromDate IS NULL OR FS."LastUpdatedAt" >= fromDate::timestamp)
                  AND (toDate IS NULL OR FS."LastUpdatedAt" <= toDate::timestamp)
                  AND (searchText IS NULL OR PS."Number" ILIKE searchText OR PS."Address" ILIKE searchText OR F."Code" ILIKE searchText)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Whether the observer has a submission with answers on a published form at the polling station in a round.
                CREATE OR REPLACE FUNCTION "HasObserverFormSubmissionsAtPollingStation"(
                    electionRoundId UUID,
                    observerId UUID,
                    pollingStationId UUID
                )
                    RETURNS BOOLEAN
                AS
                $$
                SELECT EXISTS (
                    SELECT 1
                    FROM "FormSubmissions" FS
                        INNER JOIN "MonitoringObservers" MO ON FS."MonitoringObserverId" = MO."Id"
                        INNER JOIN "Forms" F ON FS."FormId" = F."Id"
                    WHERE FS."ElectionRoundId" = electionRoundId
                      AND MO."ObserverId" = observerId
                      AND FS."PollingStationId" = pollingStationId
                      AND F."Status" = 'Published'
                      AND JSONB_ARRAY_LENGTH(FS."Answers") > 0
                )
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The "Forms" rows of the forms available to the NGO in a round for the data source (see "GetAvailableForms").
                CREATE OR REPLACE FUNCTION "GetFormSubmissionsAvailableForms"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS SETOF "Forms"
                AS
                $$
                SELECT F.*
                FROM "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF
                    INNER JOIN "Forms" F ON F."Id" = AF."FormId"
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The comments on a form submission of an observer of the NGO in a round, with their author's name.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionComments"(
                    electionRoundId UUID,
                    ngoId UUID,
                    submissionId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"             UUID,
                                "ElectionRoundId" UUID,
                                "SubmissionId"   UUID,
                                "QuestionId"     UUID,
                                "Text"           TEXT,
                                "CreatedBy"      UUID,
                                "CreatedByName"  TEXT,
                                "CreatedAt"      TIMESTAMP WITH TIME ZONE,
                                "LastModifiedAt" TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT C."Id",
                       C."ElectionRoundId",
                       C."SubmissionId",
                       C."QuestionId",
                       C."Text"::text,
                       C."CreatedBy",
                       COALESCE(U."DisplayName", '') AS "CreatedByName",
                       C."CreatedOn" AS "CreatedAt",
                       C."LastModifiedOn" AS "LastModifiedAt"
                FROM "FormSubmissionComments" C
                    INNER JOIN "FormSubmissions" FS ON FS."Id" = C."SubmissionId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = C."CreatedBy"
                WHERE C."ElectionRoundId" = electionRoundId
                  AND C."SubmissionId" = submissionId
                  AND MN."NgoId" = ngoId
                  AND MN."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            // quick-reports
            migrationBuilder.Sql(
                """
                -- The quick reports an NGO sees in a round (V1 list), with the NGO's review workflow and severity, filtered by the
                -- list's filters; a null filter (or an empty tags/severities array, or an empty search text) doesn't filter.
                CREATE OR REPLACE FUNCTION "GetQuickReportOverviews"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    coalitionMemberId UUID,
                    monitoringObserverId UUID,
                    tagsFilter TEXT[],
                    followUpStatus TEXT,
                    stageId UUID,
                    reviewStatus TEXT,
                    reviewerId UUID,
                    hasReviewer BOOLEAN,
                    severities TEXT[],
                    quickReportLocationType TEXT,
                    incidentCategory TEXT,
                    searchText TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationId UUID,
                    fromDate TEXT,
                    toDate TEXT,
                    hasAttachments BOOLEAN,
                    hasComments BOOLEAN
                )
                    RETURNS TABLE
                            (
                                "Id"                      UUID,
                                "QuickReportLocationType" TEXT,
                                "Timestamp"               TIMESTAMP WITH TIME ZONE,
                                "Title"                   TEXT,
                                "Description"             TEXT,
                                "IncidentCategory"        TEXT,
                                "FollowUpStatus"          TEXT,
                                "NumberOfAttachments"     BIGINT,
                                "CommentsCount"           BIGINT,
                                "MonitoringObserverId"    UUID,
                                "ObserverName"            TEXT,
                                "PhoneNumber"             TEXT,
                                "Email"                   TEXT,
                                "Tags"                    TEXT[],
                                "NgoName"                 TEXT,
                                "PollingStationDetails"   TEXT,
                                "PollingStationId"        UUID,
                                "Level1"                  TEXT,
                                "Level2"                  TEXT,
                                "Level3"                  TEXT,
                                "Level4"                  TEXT,
                                "Level5"                  TEXT,
                                "Number"                  TEXT,
                                "Address"                 TEXT,
                                "StageId"                 UUID,
                                "StageName"               TEXT,
                                "ReviewStatus"            TEXT,
                                "StageChangedAt"          TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"              TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"             UUID[],
                                "Severity"                TEXT
                            )
                AS
                $$
                SELECT QR."Id",
                       QR."QuickReportLocationType",
                       QR."LastUpdatedAt",
                       QR."Title"::text,
                       QR."Description"::text,
                       QR."IncidentCategory",
                       QR."FollowUpStatus",
                       (SELECT COUNT(*)
                        FROM "QuickReportAttachments" QRA
                        WHERE QRA."QuickReportId" = QR."Id"
                          AND QR."MonitoringObserverId" = QRA."MonitoringObserverId"
                          AND QRA."IsDeleted" = FALSE
                          AND QRA."IsCompleted" = TRUE),
                       (SELECT COUNT(*)
                        FROM "QuickReportComments" QRC
                            INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = QRC."CreatedBy"
                                AND NA."NgoId" = ngoId
                        WHERE QRC."QuickReportId" = QR."Id"),
                       AMO."MonitoringObserverId",
                       AMO."DisplayName",
                       AMO."PhoneNumber",
                       AMO."Email",
                       AMO."Tags",
                       AMO."NgoName"::text,
                       QR."PollingStationDetails"::text,
                       PS."Id",
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Number"::text,
                       PS."Address"::text,
                       WF."StageId",
                       WF."StageName",
                       WF."ReviewStatus",
                       WF."StageChangedAt",
                       WF."ReviewedAt",
                       WF."ReviewerIds",
                       SV."Severity"
                FROM "QuickReports" QR
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                        ON AMO."MonitoringObserverId" = QR."MonitoringObserverId"
                    LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
                    LEFT JOIN LATERAL "GetRecordReviewWorkflow"(electionRoundId, ngoId, 'QuickReport', QR."Id") WF ON TRUE
                    LEFT JOIN LATERAL "GetRecordSeverity"(electionRoundId, ngoId, 'QuickReport', QR."Id") SV ON TRUE
                WHERE QR."ElectionRoundId" = electionRoundId
                  AND (coalitionMemberId IS NULL OR AMO."NgoId" = coalitionMemberId)
                  AND (monitoringObserverId IS NULL OR AMO."MonitoringObserverId" = monitoringObserverId)
                  AND (tagsFilter IS NULL OR cardinality(tagsFilter) = 0 OR AMO."Tags" && tagsFilter)
                  AND (followUpStatus IS NULL OR QR."FollowUpStatus" = followUpStatus)
                  AND (stageId IS NULL OR WF."StageId" = stageId)
                  AND (reviewStatus IS NULL OR WF."ReviewStatus" = reviewStatus)
                  AND (reviewerId IS NULL OR reviewerId = ANY (WF."ReviewerIds"))
                  AND (hasReviewer IS NULL OR (cardinality(WF."ReviewerIds") > 0) = hasReviewer)
                  AND (severities IS NULL OR cardinality(severities) = 0 OR SV."Severity" = ANY (severities))
                  AND (quickReportLocationType IS NULL OR QR."QuickReportLocationType" = quickReportLocationType)
                  AND (incidentCategory IS NULL OR QR."IncidentCategory" = incidentCategory)
                  AND (searchText IS NULL
                    OR searchText = ''
                    OR AMO."DisplayName" ILIKE searchText
                    OR AMO."Email" ILIKE searchText
                    OR AMO."PhoneNumber" ILIKE searchText
                    OR AMO."MonitoringObserverId"::TEXT ILIKE searchText)
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (pollingStationId IS NULL OR QR."PollingStationId" = pollingStationId)
                  AND (fromDate IS NULL OR QR."LastUpdatedAt" >= fromDate::timestamp)
                  AND (toDate IS NULL OR QR."LastUpdatedAt" <= toDate::timestamp)
                  AND (hasAttachments IS NULL
                    OR ((SELECT COUNT(1)
                         FROM "QuickReportAttachments" QRA
                         WHERE QRA."QuickReportId" = QR."Id"
                           AND QRA."IsDeleted" = FALSE
                           AND QRA."IsCompleted" = TRUE) = 0 AND hasAttachments = FALSE)
                    OR ((SELECT COUNT(1)
                         FROM "QuickReportAttachments" QRA
                         WHERE QRA."QuickReportId" = QR."Id"
                           AND QRA."IsDeleted" = FALSE
                           AND QRA."IsCompleted" = TRUE) > 0 AND hasAttachments = TRUE))
                  AND (hasComments IS NULL
                    OR ((SELECT COUNT(1)
                         FROM "QuickReportComments" QRC
                             INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = QRC."CreatedBy"
                                 AND NA."NgoId" = ngoId
                         WHERE QRC."QuickReportId" = QR."Id") = 0 AND hasComments = FALSE)
                    OR ((SELECT COUNT(1)
                         FROM "QuickReportComments" QRC
                             INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = QRC."CreatedBy"
                                 AND NA."NgoId" = ngoId
                         WHERE QRC."QuickReportId" = QR."Id") > 0 AND hasComments = TRUE))
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- A quick report as an NGO sees it (observers available to the NGO, own and coalition), with its completed
                -- attachments and its polling station corrections. No row when the NGO can't see it.
                CREATE OR REPLACE FUNCTION "GetNgoQuickReportDetails"(
                    electionRoundId UUID,
                    ngoId UUID,
                    quickReportId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"                        UUID,
                                "ElectionRoundId"           UUID,
                                "QuickReportLocationType"   TEXT,
                                "Timestamp"                 TIMESTAMP WITH TIME ZONE,
                                "Title"                     TEXT,
                                "Description"               TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "PhoneNumber"               TEXT,
                                "Email"                     TEXT,
                                "Tags"                      TEXT[],
                                "NgoName"                   TEXT,
                                "IsOwnObserver"             BOOLEAN,
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "Number"                    TEXT,
                                "Address"                   TEXT,
                                "PollingStationDetails"     TEXT,
                                "IncidentCategory"          TEXT,
                                "FollowUpStatus"            TEXT,
                                "Attachments"               JSONB,
                                "PollingStationCorrections" JSONB
                            )
                AS
                $$
                SELECT QR."Id",
                       QR."ElectionRoundId",
                       QR."QuickReportLocationType",
                       QR."LastUpdatedAt",
                       QR."Title"::text,
                       QR."Description"::text,
                       QR."MonitoringObserverId",
                       AMO."DisplayName",
                       AMO."PhoneNumber",
                       AMO."Email",
                       AMO."Tags",
                       AMO."NgoName"::text,
                       AMO."IsOwnObserver",
                       QR."PollingStationId",
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Number"::text,
                       PS."Address"::text,
                       QR."PollingStationDetails"::text,
                       QR."IncidentCategory",
                       QR."FollowUpStatus",
                       COALESCE((SELECT jsonb_agg(jsonb_build_object('QuickReportId', QR."Id", 'FileName', QRA."FileName", 'MimeType', QRA."MimeType", 'FilePath', QRA."FilePath", 'UploadedFileName', QRA."UploadedFileName", 'TimeSubmitted', QRA."LastUpdatedAt"))
                                 FROM "QuickReportAttachments" QRA
                                 WHERE QRA."ElectionRoundId" = electionRoundId
                                   AND QRA."MonitoringObserverId" = QR."MonitoringObserverId"
                                   AND QRA."QuickReportId" = quickReportId
                                   AND QRA."IsDeleted" = FALSE
                                   AND QRA."IsCompleted" = TRUE), '[]'::JSONB),
                       COALESCE((SELECT jsonb_agg(jsonb_build_object(
                                                          'CorrectedAt', to_char(PSC."CorrectedAt" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS.US"Z"'),
                                                          'From', jsonb_build_object('Id', FPS."Id", 'Number', FPS."Number", 'Level1', FPS."Level1", 'Level2', FPS."Level2", 'Level3', FPS."Level3", 'Level4', FPS."Level4", 'Level5', FPS."Level5", 'Address', FPS."Address"),
                                                          'To', jsonb_build_object('Id', TPS."Id", 'Number', TPS."Number", 'Level1', TPS."Level1", 'Level2', TPS."Level2", 'Level3', TPS."Level3", 'Level4', TPS."Level4", 'Level5', TPS."Level5", 'Address', TPS."Address")
                                                  ) ORDER BY PSC."CorrectedAt")
                                 FROM "PollingStationCorrections" PSC
                                     INNER JOIN "PollingStations" FPS ON FPS."Id" = PSC."FromPollingStationId"
                                     INNER JOIN "PollingStations" TPS ON TPS."Id" = PSC."ToPollingStationId"
                                 WHERE PSC."RecordType" = 'QuickReport'
                                   AND PSC."RecordId" = QR."Id"), '[]'::JSONB)
                FROM "QuickReports" QR
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO
                        ON AMO."MonitoringObserverId" = QR."MonitoringObserverId"
                    LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
                WHERE QR."ElectionRoundId" = electionRoundId
                  AND QR."Id" = quickReportId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- A quick report of an observer (by the observer's id), with its completed attachments. No row when it isn't theirs.
                CREATE OR REPLACE FUNCTION "GetObserverQuickReport"(
                    electionRoundId UUID,
                    observerId UUID,
                    quickReportId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"                      UUID,
                                "ElectionRoundId"         UUID,
                                "QuickReportLocationType" TEXT,
                                "Timestamp"               TIMESTAMP WITH TIME ZONE,
                                "Title"                   TEXT,
                                "Description"             TEXT,
                                "PollingStationId"        UUID,
                                "Level1"                  TEXT,
                                "Level2"                  TEXT,
                                "Level3"                  TEXT,
                                "Level4"                  TEXT,
                                "Level5"                  TEXT,
                                "Number"                  TEXT,
                                "Address"                 TEXT,
                                "PollingStationDetails"   TEXT,
                                "IncidentCategory"        TEXT,
                                "FollowUpStatus"          TEXT,
                                "Attachments"             JSONB
                            )
                AS
                $$
                SELECT QR."Id",
                       QR."ElectionRoundId",
                       QR."QuickReportLocationType",
                       QR."LastUpdatedAt",
                       QR."Title"::text,
                       QR."Description"::text,
                       QR."PollingStationId",
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Number"::text,
                       PS."Address"::text,
                       QR."PollingStationDetails"::text,
                       QR."IncidentCategory",
                       QR."FollowUpStatus",
                       COALESCE((SELECT jsonb_agg(jsonb_build_object('QuickReportId', QR."Id", 'FileName', QRA."FileName", 'MimeType', QRA."MimeType", 'FilePath', QRA."FilePath", 'UploadedFileName', QRA."UploadedFileName", 'TimeSubmitted', QRA."LastUpdatedAt"))
                                 FROM "QuickReportAttachments" QRA
                                 WHERE QRA."ElectionRoundId" = electionRoundId
                                   AND QRA."MonitoringObserverId" = QR."MonitoringObserverId"
                                   AND QRA."QuickReportId" = quickReportId
                                   AND QRA."IsDeleted" = FALSE
                                   AND QRA."IsCompleted" = TRUE), '[]'::JSONB)
                FROM "QuickReports" QR
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                    LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
                WHERE QR."ElectionRoundId" = electionRoundId
                  AND QR."Id" = quickReportId
                  AND MO."ObserverId" = observerId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- All quick reports of a round with their NGO and (unmasked) observer data, for platform admins; a null filter
                -- doesn't filter. searchText is an ILIKE pattern.
                CREATE OR REPLACE FUNCTION "GetPlatformAdminQuickReports"(
                    electionRoundId UUID,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    monitoringNgoId UUID,
                    fromDate TEXT,
                    toDate TEXT,
                    searchText TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                      UUID,
                                "Timestamp"               TIMESTAMP WITH TIME ZONE,
                                "Title"                   TEXT,
                                "IncidentCategory"        TEXT,
                                "QuickReportLocationType" TEXT,
                                "PollingStationDetails"   TEXT,
                                "PollingStationId"        UUID,
                                "Level1"                  TEXT,
                                "Level2"                  TEXT,
                                "Level3"                  TEXT,
                                "Level4"                  TEXT,
                                "Level5"                  TEXT,
                                "Number"                  TEXT,
                                "Address"                 TEXT,
                                "MonitoringNgoId"         UUID,
                                "NgoName"                 TEXT,
                                "MonitoringObserverId"    UUID,
                                "ObserverName"            TEXT,
                                "ObserverEmail"           TEXT,
                                "ObserverPhoneNumber"     TEXT,
                                "FollowUpStatus"          TEXT
                            )
                AS
                $$
                SELECT QR."Id",
                       QR."LastUpdatedAt",
                       QR."Title"::text,
                       QR."IncidentCategory",
                       QR."QuickReportLocationType",
                       QR."PollingStationDetails"::text,
                       PS."Id",
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Number"::text,
                       PS."Address"::text,
                       MN."Id",
                       N."Name"::text,
                       MO."Id",
                       U."DisplayName",
                       U."Email"::text,
                       U."PhoneNumber",
                       QR."FollowUpStatus"
                FROM "QuickReports" QR
                    LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE QR."ElectionRoundId" = electionRoundId
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                  AND (fromDate IS NULL OR QR."LastUpdatedAt" >= fromDate::timestamp)
                  AND (toDate IS NULL OR QR."LastUpdatedAt" <= toDate::timestamp)
                  AND (searchText IS NULL OR QR."Title" ILIKE searchText OR PS."Number" ILIKE searchText
                    OR PS."Address" ILIKE searchText OR QR."PollingStationDetails" ILIKE searchText)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The first and last submission time of the quick reports from the observers available to an NGO in a round
                -- (nulls when there are none). Always one row.
                CREATE OR REPLACE FUNCTION "GetQuickReportsTimestampFilterOptions"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "FirstSubmissionTimestamp" TIMESTAMP WITH TIME ZONE,
                                "LastSubmissionTimestamp"  TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT MIN(QR."LastUpdatedAt"),
                       MAX(QR."LastUpdatedAt")
                FROM "QuickReports" QR
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO
                        ON QR."MonitoringObserverId" = MO."MonitoringObserverId"
                WHERE QR."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The observers available to an NGO in a round who submitted quick reports, once each.
                CREATE OR REPLACE FUNCTION "GetQuickReportsObserverFilterOptions"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "MonitoringObserverId" UUID,
                                "DisplayName"          TEXT,
                                "Email"                TEXT,
                                "AccountStatus"        TEXT
                            )
                AS
                $$
                SELECT DISTINCT MO."MonitoringObserverId",
                                MO."DisplayName",
                                MO."Email",
                                MO."AccountStatus"
                FROM "QuickReports" QR
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO
                        ON QR."MonitoringObserverId" = MO."MonitoringObserverId"
                WHERE QR."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The comments on a quick report of an NGO's observer, with their authors' names ('' when unknown).
                CREATE OR REPLACE FUNCTION "GetQuickReportComments"(
                    electionRoundId UUID,
                    ngoId UUID,
                    quickReportId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"              UUID,
                                "ElectionRoundId" UUID,
                                "QuickReportId"   UUID,
                                "Text"            TEXT,
                                "CreatedBy"       UUID,
                                "CreatedByName"   TEXT,
                                "CreatedAt"       TIMESTAMP WITH TIME ZONE,
                                "LastModifiedAt"  TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT C."Id",
                       C."ElectionRoundId",
                       C."QuickReportId",
                       C."Text"::text,
                       C."CreatedBy",
                       COALESCE(U."DisplayName", ''),
                       C."CreatedOn",
                       C."LastModifiedOn"
                FROM "QuickReportComments" C
                    INNER JOIN "QuickReports" QR ON QR."Id" = C."QuickReportId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = QR."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = C."CreatedBy"
                WHERE C."ElectionRoundId" = electionRoundId
                  AND C."QuickReportId" = quickReportId
                  AND MN."NgoId" = ngoId
                  AND MN."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            // incident-reports
            migrationBuilder.Sql(
                """
                -- Counts the NGO's incident reports matching the incident reports list (by entry) filters.
                CREATE OR REPLACE FUNCTION "GetIncidentReportListEntriesCount"(
                    electionRoundId UUID,
                    ngoId UUID,
                    monitoringObserverId UUID,
                    searchText TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    pollingStationId UUID,
                    hasFlaggedAnswers BOOLEAN,
                    followUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    formStatus TEXT,
                    questionsAnswered TEXT,
                    locationType TEXT,
                    hasAttachments BOOLEAN,
                    hasNotes BOOLEAN,
                    hasComments BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    stageId UUID,
                    reviewStatus TEXT,
                    reviewerId UUID,
                    hasReviewer BOOLEAN,
                    severities TEXT[]
                )
                    RETURNS TABLE
                            (
                                "Count" BIGINT
                            )
                AS
                $$
                SELECT
                    COUNT(*)
                FROM
                    "IncidentReports" IR
                        INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                        INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                        LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                LEFT JOIN LATERAL "GetRecordReviewWorkflow"(electionRoundId, ngoId, 'IncidentReport', IR."Id") WF ON TRUE
                LEFT JOIN LATERAL "GetRecordSeverity"(electionRoundId, ngoId, 'IncidentReport', IR."Id") SV ON TRUE
                WHERE
                    MN."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                  AND (monitoringObserverId IS NULL OR MO."Id" = monitoringObserverId)
                  AND (searchText IS NULL
                        OR searchText = ''
                        OR U."DisplayName" ILIKE searchText
                        OR U."Email" ILIKE searchText
                        OR U."PhoneNumber" ILIKE searchText
                        OR MO."Id"::TEXT ILIKE searchText
                    )
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (pollingStationNumber IS NULL OR PS."Number" = pollingStationNumber)
                  AND (pollingStationId IS NULL OR IR."PollingStationId" = pollingStationId)
                  AND (hasFlaggedAnswers IS NULL
                        OR (IR."NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                        OR ("NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE)
                    )
                  AND (followUpStatus IS NULL OR IR."FollowUpStatus" = followUpStatus)
                  AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR MO."Tags" && tagsFilter)
                  AND (monitoringObserverStatus IS NULL OR MO."Status" = monitoringObserverStatus)
                  AND (formId IS NULL OR IR."FormId" = formId)
                  AND (formStatus IS NULL OR F."Status" = formStatus)
                  AND (questionsAnswered IS NULL
                        OR (questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                        OR (questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                        OR (questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                    )
                  AND (locationType IS NULL OR IR."LocationType" = locationType)
                  AND (hasAttachments IS NULL
                        OR (hasAttachments = FALSE AND
                        (
                            SELECT
                                COUNT(1)
                            FROM
                                "IncidentReportAttachments" A
                            WHERE
                                A."IncidentReportId" = IR."Id"
                              AND IR."PollingStationId" = "PollingStationId"
                              AND A."IsDeleted" = FALSE
                              AND A."IsCompleted" = TRUE
                        ) = 0)
                        OR (
                        (hasAttachments = TRUE AND(
                            SELECT
                                COUNT(1)
                            FROM
                                "IncidentReportAttachments" A
                            WHERE
                                A."IncidentReportId" = IR."Id"
                              AND A."IsDeleted" = FALSE
                              AND A."IsCompleted" = TRUE
                        ) > 0)
                    )
                  AND (hasNotes IS NULL
                        OR (hasNotes = FALSE AND (SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") = 0)
                        OR (hasNotes = TRUE AND (SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") > 0 )
                    ))
                  AND (hasComments IS NULL
                        OR (hasComments = FALSE AND (SELECT COUNT(1) FROM "IncidentReportComments" ICC INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = ICC."CreatedBy" AND NA."NgoId" = ngoId WHERE ICC."IncidentReportId" = IR."Id") = 0)
                        OR (hasComments = TRUE AND (SELECT COUNT(1) FROM "IncidentReportComments" ICC INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = ICC."CreatedBy" AND NA."NgoId" = ngoId WHERE ICC."IncidentReportId" = IR."Id") > 0)
                    )
                  AND (fromDate is NULL OR IR."LastUpdatedAt" >= fromDate::timestamp)
                  AND (stageId IS NULL OR WF."StageId" = stageId)
                  AND (reviewStatus IS NULL OR WF."ReviewStatus" = reviewStatus)
                  AND (reviewerId IS NULL OR reviewerId = ANY(WF."ReviewerIds"))
                  AND (hasReviewer IS NULL OR (cardinality(WF."ReviewerIds") > 0) = hasReviewer)
                  AND (severities IS NULL OR cardinality(severities) = 0 OR SV."Severity" = ANY(severities))
                  AND (toDate is NULL OR IR."LastUpdatedAt" <= toDate::timestamp)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- One page of the NGO's incident reports for the incident reports list (by entry), with each report's review
                -- workflow and severity, filtered and ordered by sortExpression ('<Column> ASC|DESC').
                CREATE OR REPLACE FUNCTION "GetIncidentReportListEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    monitoringObserverId UUID,
                    searchText TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    pollingStationId UUID,
                    hasFlaggedAnswers BOOLEAN,
                    followUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    formStatus TEXT,
                    questionsAnswered TEXT,
                    locationType TEXT,
                    hasAttachments BOOLEAN,
                    hasNotes BOOLEAN,
                    hasComments BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    stageId UUID,
                    reviewStatus TEXT,
                    reviewerId UUID,
                    hasReviewer BOOLEAN,
                    severities TEXT[],
                    sortExpression TEXT,
                    offsetRows INTEGER,
                    pageSize INTEGER
                )
                    RETURNS TABLE
                            (
                                "IncidentReportId"          UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormCode"                  TEXT,
                                "FormStatus"                TEXT,
                                "FormName"                  JSONB,
                                "FormDefaultLanguage"       TEXT,
                                "LocationType"              TEXT,
                                "LocationDescription"       TEXT,
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "PollingStationNumber"      TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "Email"                     TEXT,
                                "PhoneNumber"               TEXT,
                                "Status"                    TEXT,
                                "Tags"                      TEXT[],
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "MediaFilesCount"           BIGINT,
                                "NotesCount"                BIGINT,
                                "CommentsCount"             BIGINT,
                                "FollowUpStatus"            TEXT,
                                "IsCompleted"               BOOLEAN,
                                "StageId"                   UUID,
                                "StageName"                 TEXT,
                                "ReviewStatus"              TEXT,
                                "StageChangedAt"            TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"                TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"               UUID[],
                                "Severity"                  TEXT
                            )
                AS
                $$
                WITH
                    INCIDENT_REPORTS AS (
                        SELECT
                            IR."Id" AS "IncidentReportId",
                            F."Code" AS "FormCode",
                            F."Status" AS "FormStatus",
                            F."Name" AS "FormName",
                            F."DefaultLanguage" "FormDefaultLanguage",
                            IR."LocationType",
                            IR."LocationDescription",
                            IR."PollingStationId",
                            IR."MonitoringObserverId",
                            IR."NumberOfQuestionsAnswered",
                            IR."NumberOfFlaggedAnswers",
                            (
                                SELECT
                                    COUNT(1)
                                FROM
                                    "IncidentReportAttachments" A
                                WHERE
                                    A."IncidentReportId" = IR."Id"
                                  AND A."IsDeleted" = FALSE
                                  AND A."IsCompleted" = TRUE
                            ) AS "MediaFilesCount",
                            ( SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") AS "NotesCount",
                            ( SELECT COUNT(1)
                              FROM "IncidentReportComments" ICC
                                  INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = ICC."CreatedBy"
                                      AND NA."NgoId" = ngoId
                              WHERE ICC."IncidentReportId" = IR."Id") AS "CommentsCount",
                            IR."LastUpdatedAt" AS "TimeSubmitted",
                            IR."FollowUpStatus",
                            IR."IsCompleted",
                            WF."StageId", WF."StageName", WF."ReviewStatus", WF."StageChangedAt", WF."ReviewedAt", WF."ReviewerIds", SV."Severity"
                        FROM
                            "IncidentReports" IR
                                INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                                INNER JOIN "MonitoringObservers" MO ON IR."MonitoringObserverId" = MO."Id"
                                INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        LEFT JOIN LATERAL "GetRecordReviewWorkflow"(electionRoundId, ngoId, 'IncidentReport', IR."Id") WF ON TRUE
                        LEFT JOIN LATERAL "GetRecordSeverity"(electionRoundId, ngoId, 'IncidentReport', IR."Id") SV ON TRUE
                        WHERE
                          MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                          AND (monitoringObserverId IS NULL OR MO."Id" = monitoringObserverId)
                          AND (monitoringObserverStatus IS NULL OR MO."Status" = monitoringObserverStatus)
                          AND (formId IS NULL OR IR."FormId" = formId)
                          AND (formStatus IS NULL OR F."Status" = formStatus)
                          AND (locationType IS NULL OR IR."LocationType" = locationType)
                          AND (questionsAnswered IS NULL
                              OR (questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                              OR (questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                              OR (questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                          )
                          AND (fromDate is NULL OR IR."LastUpdatedAt" >= fromDate::timestamp)
                          AND (stageId IS NULL OR WF."StageId" = stageId)
                          AND (reviewStatus IS NULL OR WF."ReviewStatus" = reviewStatus)
                          AND (reviewerId IS NULL OR reviewerId = ANY(WF."ReviewerIds"))
                          AND (hasReviewer IS NULL OR (cardinality(WF."ReviewerIds") > 0) = hasReviewer)
                          AND (severities IS NULL OR cardinality(severities) = 0 OR SV."Severity" = ANY(severities))
                          AND (toDate is NULL OR IR."LastUpdatedAt" <= toDate::timestamp)
                    )
                SELECT
                    IR."IncidentReportId",
                    IR."TimeSubmitted",
                    IR."FormCode"::text,
                    IR."FormStatus",
                    IR."FormName",
                    IR."FormDefaultLanguage"::text,
                    IR."LocationType",
                    IR."LocationDescription"::text,
                    PS."Id" AS "PollingStationId",
                    PS."Level1"::text,
                    PS."Level2"::text,
                    PS."Level3"::text,
                    PS."Level4"::text,
                    PS."Level5"::text,
                    PS."Number"::text "PollingStationNumber",
                    IR."MonitoringObserverId",
                    U."DisplayName" AS "ObserverName",
                    U."Email"::text,
                    U."PhoneNumber",
                    MO."Status",
                    MO."Tags",
                    IR."NumberOfQuestionsAnswered",
                    IR."NumberOfFlaggedAnswers",
                    IR."MediaFilesCount",
                    IR."NotesCount",
                    IR."CommentsCount",
                    IR."FollowUpStatus",
                    IR."IsCompleted",
                    IR."StageId", IR."StageName", IR."ReviewStatus", IR."StageChangedAt", IR."ReviewedAt", IR."ReviewerIds", IR."Severity"
                FROM
                    INCIDENT_REPORTS IR
                        INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                        LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                WHERE
                    MN."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                  AND (monitoringObserverId IS NULL OR MO."Id" = monitoringObserverId)
                  AND (
                    searchText IS NULL
                        OR searchText = ''
                        OR U."DisplayName" ILIKE searchText
                        OR U."Email" ILIKE searchText
                        OR U."PhoneNumber" ILIKE searchText
                        OR MO."Id"::TEXT ILIKE searchText
                    )
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (pollingStationNumber IS NULL OR PS."Number" = pollingStationNumber)
                  AND (pollingStationId IS NULL OR IR."PollingStationId" = pollingStationId)
                  AND (hasFlaggedAnswers IS NULL
                    OR (IR."NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                    OR (IR."NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE)
                    )
                  AND (followUpStatus IS NULL OR IR."FollowUpStatus" = followUpStatus)
                  AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR MO."Tags" && tagsFilter)
                  AND (hasNotes IS NULL
                    OR (IR."NotesCount" = 0 AND hasNotes = FALSE)
                    OR (IR."NotesCount" > 0 AND hasNotes = TRUE)
                    )
                  AND (hasAttachments IS NULL
                    OR (IR."MediaFilesCount" = 0 AND hasAttachments = FALSE)
                    OR (IR."MediaFilesCount" > 0 AND hasAttachments = TRUE)
                    )
                  AND (hasComments IS NULL
                    OR (IR."CommentsCount" = 0 AND hasComments = FALSE)
                    OR (IR."CommentsCount" > 0 AND hasComments = TRUE)
                    )
                ORDER BY
                    CASE WHEN sortExpression = 'Severity ASC' THEN "SeverityRank"(IR."Severity") END ASC,
                    CASE WHEN sortExpression = 'Severity DESC' THEN "SeverityRank"(IR."Severity") END DESC,
                    CASE WHEN sortExpression LIKE 'Severity %' THEN IR."TimeSubmitted" END DESC,
                    CASE WHEN sortExpression = 'TimeSubmitted ASC' THEN IR."TimeSubmitted" END ASC,
                    CASE WHEN sortExpression = 'TimeSubmitted DESC' THEN IR."TimeSubmitted" END DESC,
                    CASE WHEN sortExpression = 'FormCode ASC' THEN IR."FormCode" END ASC,
                    CASE WHEN sortExpression = 'FormCode DESC' THEN IR."FormCode" END DESC,
                    CASE WHEN sortExpression = 'NumberOfQuestionsAnswered ASC' THEN IR."NumberOfQuestionsAnswered" END ASC,
                    CASE WHEN sortExpression = 'NumberOfQuestionsAnswered DESC' THEN IR."NumberOfQuestionsAnswered" END DESC,
                    CASE WHEN sortExpression = 'NumberOfFlaggedAnswers ASC' THEN IR."NumberOfFlaggedAnswers" END ASC,
                    CASE WHEN sortExpression = 'NumberOfFlaggedAnswers DESC' THEN IR."NumberOfFlaggedAnswers" END DESC,
                    CASE WHEN sortExpression = 'MediaFilesCount ASC' THEN IR."MediaFilesCount" END ASC,
                    CASE WHEN sortExpression = 'MediaFilesCount DESC' THEN IR."MediaFilesCount" END DESC,
                    CASE WHEN sortExpression = 'NotesCount ASC' THEN IR."NotesCount" END ASC,
                    CASE WHEN sortExpression = 'NotesCount DESC' THEN IR."NotesCount" END DESC,
                    CASE WHEN sortExpression = 'Level1 ASC' THEN PS."Level1" END ASC,
                    CASE WHEN sortExpression = 'Level1 DESC' THEN PS."Level1" END DESC,
                    CASE WHEN sortExpression = 'Level2 ASC' THEN PS."Level2" END ASC,
                    CASE WHEN sortExpression = 'Level2 DESC' THEN PS."Level2" END DESC,
                    CASE WHEN sortExpression = 'Level3 ASC' THEN PS."Level3" END ASC,
                    CASE WHEN sortExpression = 'Level3 DESC' THEN PS."Level3" END DESC,
                    CASE WHEN sortExpression = 'Level4 ASC' THEN PS."Level4" END ASC,
                    CASE WHEN sortExpression = 'Level4 DESC' THEN PS."Level4" END DESC,
                    CASE WHEN sortExpression = 'Level5 ASC' THEN PS."Level5" END ASC,
                    CASE WHEN sortExpression = 'Level5 DESC' THEN PS."Level5" END DESC,
                    CASE WHEN sortExpression = 'PollingStationNumber ASC' THEN PS."Number" END ASC,
                    CASE WHEN sortExpression = 'PollingStationNumber DESC' THEN PS."Number" END DESC,
                    CASE WHEN sortExpression = 'ObserverName ASC' THEN U."DisplayName" END ASC,
                    CASE WHEN sortExpression = 'ObserverName DESC' THEN U."DisplayName" END DESC
                OFFSET offsetRows ROWS
                FETCH NEXT pageSize ROWS ONLY
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The incident reports of an election round for platform admins, with the observer's (unmasked) data, NGO and
                -- polling station. All filters are optional.
                CREATE OR REPLACE FUNCTION "GetIncidentReportsForPlatformAdmin"(
                    electionRoundId UUID,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    monitoringNgoId UUID,
                    formId UUID,
                    isCompleted BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    searchText TEXT
                )
                    RETURNS TABLE
                            (
                                "IncidentReportId"          UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormId"                    UUID,
                                "FormCode"                  TEXT,
                                "FormName"                  JSONB,
                                "FormDefaultLanguage"       TEXT,
                                "LocationType"              TEXT,
                                "LocationDescription"       TEXT,
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "Number"                    TEXT,
                                "Address"                   TEXT,
                                "MonitoringNgoId"           UUID,
                                "NgoName"                   TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "ObserverEmail"             TEXT,
                                "ObserverPhoneNumber"       TEXT,
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "FollowUpStatus"            TEXT,
                                "IsCompleted"               BOOLEAN
                            )
                AS
                $$
                SELECT
                    IR."Id" AS "IncidentReportId",
                    IR."LastUpdatedAt" AS "TimeSubmitted",
                    F."Id" AS "FormId",
                    F."Code"::text AS "FormCode",
                    F."Name" AS "FormName",
                    F."DefaultLanguage"::text AS "FormDefaultLanguage",
                    IR."LocationType",
                    IR."LocationDescription"::text,
                    PS."Id" AS "PollingStationId",
                    PS."Level1"::text,
                    PS."Level2"::text,
                    PS."Level3"::text,
                    PS."Level4"::text,
                    PS."Level5"::text,
                    PS."Number"::text,
                    PS."Address"::text,
                    MN."Id" AS "MonitoringNgoId",
                    N."Name"::text AS "NgoName",
                    MO."Id" AS "MonitoringObserverId",
                    U."DisplayName" AS "ObserverName",
                    U."Email"::text AS "ObserverEmail",
                    U."PhoneNumber" AS "ObserverPhoneNumber",
                    IR."NumberOfQuestionsAnswered",
                    IR."NumberOfFlaggedAnswers",
                    IR."FollowUpStatus",
                    IR."IsCompleted"
                FROM "IncidentReports" IR
                    INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                    LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE IR."ElectionRoundId" = electionRoundId
                    AND (level1 IS NULL OR PS."Level1" = level1)
                    AND (level2 IS NULL OR PS."Level2" = level2)
                    AND (level3 IS NULL OR PS."Level3" = level3)
                    AND (level4 IS NULL OR PS."Level4" = level4)
                    AND (level5 IS NULL OR PS."Level5" = level5)
                    AND (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                    AND (formId IS NULL OR IR."FormId" = formId)
                    AND (isCompleted IS NULL OR IR."IsCompleted" = isCompleted)
                    AND (fromDate IS NULL OR IR."LastUpdatedAt" >= fromDate::timestamp)
                    AND (toDate IS NULL OR IR."LastUpdatedAt" <= toDate::timestamp)
                    AND (searchText IS NULL OR PS."Number" ILIKE searchText OR PS."Address" ILIKE searchText
                         OR IR."LocationDescription" ILIKE searchText OR F."Code" ILIKE searchText)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The first and last update times of the incident reports from the observers available to the NGO for the data source.
                CREATE OR REPLACE FUNCTION "GetIncidentReportTimestampsFilterOptions"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "FirstSubmissionTimestamp" TIMESTAMP WITH TIME ZONE,
                                "LastSubmissionTimestamp"  TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT MIN(IR."LastUpdatedAt") AS "FirstSubmissionTimestamp",
                       MAX(IR."LastUpdatedAt") AS "LastSubmissionTimestamp"
                FROM "IncidentReports" IR
                INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO
                  ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                WHERE IR."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The distinct forms of the incident reports from the observers available to the NGO for the data source.
                CREATE OR REPLACE FUNCTION "GetIncidentReportFormFilterOptions"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "FormId"          UUID,
                                "FormName"        JSONB,
                                "FormCode"        TEXT,
                                "DefaultLanguage" TEXT
                            )
                AS
                $$
                SELECT DISTINCT
                  F."Id" AS "FormId",
                  F."Name" AS "FormName",
                  F."Code"::text AS "FormCode",
                  F."DefaultLanguage"::text
                FROM "IncidentReports" IR
                INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO
                  ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                WHERE IR."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The distinct observers available to the NGO for the data source that submitted incident reports.
                CREATE OR REPLACE FUNCTION "GetIncidentReportObserverFilterOptions"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "MonitoringObserverId" UUID,
                                "DisplayName"          TEXT,
                                "Email"                TEXT,
                                "AccountStatus"        TEXT
                            )
                AS
                $$
                SELECT DISTINCT
                  MO."MonitoringObserverId",
                  MO."DisplayName",
                  MO."Email",
                  MO."AccountStatus"
                FROM "IncidentReports" IR
                INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO
                  ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                WHERE IR."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The observers available to the NGO for the data source with their incident report counts, matching the search text
                -- and tags; with needsFollowUp set, only observers with a report that needs follow-up.
                CREATE OR REPLACE FUNCTION "GetIncidentReportObserversOverview"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    searchText TEXT,
                    tagsFilter TEXT[],
                    needsFollowUp TEXT
                )
                    RETURNS TABLE
                            (
                                "MonitoringObserverId"       UUID,
                                "ObserverName"               TEXT,
                                "PhoneNumber"                TEXT,
                                "Email"                      TEXT,
                                "Tags"                       TEXT[],
                                "NumberOfFlaggedAnswers"     BIGINT,
                                "NumberOfIncidentsSubmitted" BIGINT,
                                "FollowUpStatus"             TEXT
                            )
                AS
                $$
                SELECT "MonitoringObserverId",
                       "ObserverName",
                       "PhoneNumber",
                       "Email",
                       "Tags",
                       "NumberOfFlaggedAnswers",
                       "NumberOfIncidentsSubmitted",
                       "FollowUpStatus"
                FROM (SELECT MO."MonitoringObserverId" AS "MonitoringObserverId",
                             mo."DisplayName" AS "ObserverName",
                             mo."PhoneNumber",
                             mo."Email",
                             MO."Tags",
                             COALESCE(
                                     (SELECT SUM("NumberOfFlaggedAnswers")
                                      FROM "IncidentReports" IR
                                      WHERE IR."MonitoringObserverId" = MO."MonitoringObserverId"),
                                     0
                             ) AS "NumberOfFlaggedAnswers",
                             (SELECT COUNT(1)
                              FROM "IncidentReports" IR
                              WHERE IR."MonitoringObserverId" = MO."MonitoringObserverId" AND IR."ElectionRoundId" = electionRoundId) AS "NumberOfIncidentsSubmitted",
                             (
                                 CASE
                                     WHEN EXISTS (SELECT 1
                                                  FROM "IncidentReports" IR
                                                  WHERE IR."FollowUpStatus" = 'NeedsFollowUp'
                                                    AND IR."MonitoringObserverId" = MO."MonitoringObserverId"
                                                    AND IR."ElectionRoundId" = electionRoundId)
                                         THEN 'NeedsFollowUp'
                                     ELSE NULL
                                     END
                                 ) AS "FollowUpStatus"
                      FROM "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO
                      WHERE
                          (searchText IS NULL
                                 OR searchText = ''
                                 OR MO."DisplayName" ILIKE searchText
                                 OR MO."Email" ILIKE searchText
                                 OR MO."PhoneNumber" ILIKE searchText
                                 OR MO."MonitoringObserverId"::TEXT ILIKE searchText)
                        AND (tagsFilter IS NULL OR cardinality(tagsFilter) = 0 OR mo."Tags" && tagsFilter)) T
                WHERE (needsFollowUp IS NULL OR T."FollowUpStatus" = 'NeedsFollowUp')
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The comments on an incident report of the NGO, with their authors' names.
                CREATE OR REPLACE FUNCTION "GetIncidentReportComments"(
                    electionRoundId UUID,
                    ngoId UUID,
                    incidentReportId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"               UUID,
                                "ElectionRoundId"  UUID,
                                "IncidentReportId" UUID,
                                "Text"             TEXT,
                                "CreatedBy"        UUID,
                                "CreatedByName"    TEXT,
                                "CreatedAt"        TIMESTAMP WITH TIME ZONE,
                                "LastModifiedAt"   TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT
                    c."Id",
                    c."ElectionRoundId",
                    c."IncidentReportId",
                    c."Text"::text,
                    c."CreatedBy",
                    COALESCE(u."DisplayName", '') AS "CreatedByName",
                    c."CreatedOn" AS "CreatedAt",
                    c."LastModifiedOn" AS "LastModifiedAt"
                FROM "IncidentReportComments" c
                INNER JOIN "IncidentReports" ir ON ir."Id" = c."IncidentReportId"
                INNER JOIN "MonitoringObservers" mo ON mo."Id" = ir."MonitoringObserverId"
                INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                LEFT JOIN "AspNetUsers" u ON u."Id" = c."CreatedBy"
                WHERE c."ElectionRoundId" = electionRoundId
                  AND c."IncidentReportId" = incidentReportId
                  AND mn."NgoId" = ngoId
                  AND mn."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            // citizen-reports
            migrationBuilder.Sql(
                """
                -- The citizen reports of the NGO's citizen-reporting round for the V1 by-entry list, with their counts, location
                -- levels, the NGO's review workflow and severity, filtered by the list's filters (a null filter doesn't filter; an
                -- empty severities array doesn't filter). Unordered: the endpoint sorts and pages.
                CREATE OR REPLACE FUNCTION "GetCitizenReportListEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    followUpStatus TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    locationId UUID,
                    formId UUID,
                    formStatus TEXT,
                    questionsAnswered TEXT,
                    hasAttachments BOOLEAN,
                    hasNotes BOOLEAN,
                    hasComments BOOLEAN,
                    hasFlaggedAnswers BOOLEAN,
                    stageId UUID,
                    reviewStatus TEXT,
                    reviewerId UUID,
                    hasReviewer BOOLEAN,
                    severities TEXT[]
                )
                    RETURNS TABLE
                            (
                                "CitizenReportId"           UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormCode"                  TEXT,
                                "FormStatus"                TEXT,
                                "FormName"                  JSONB,
                                "FormDefaultLanguage"       TEXT,
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "NotesCount"                BIGINT,
                                "MediaFilesCount"           BIGINT,
                                "CommentsCount"             BIGINT,
                                "FollowUpStatus"            TEXT,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "StageId"                   UUID,
                                "StageName"                 TEXT,
                                "ReviewStatus"              TEXT,
                                "StageChangedAt"            TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"                TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"               UUID[],
                                "Severity"                  TEXT
                            )
                AS
                $$
                SELECT CR."Id",
                       COALESCE(CR."LastModifiedOn", CR."CreatedOn"),
                       F."Code"::text,
                       F."Status",
                       F."Name",
                       F."DefaultLanguage"::text,
                       CR."NumberOfQuestionsAnswered",
                       CR."NumberOfFlaggedAnswers",
                       (SELECT COUNT(1)
                        FROM "CitizenReportNotes" CRN
                        WHERE CRN."CitizenReportId" = CR."Id"),
                       (SELECT COUNT(1)
                        FROM "CitizenReportAttachments" CRA
                        WHERE CRA."CitizenReportId" = CR."Id"
                          AND CRA."IsCompleted" = TRUE
                          AND CRA."IsDeleted" = FALSE),
                       (SELECT COUNT(1)
                        FROM "CitizenReportComments" CRC
                            INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = CRC."CreatedBy" AND NA."NgoId" = ngoId
                        WHERE CRC."CitizenReportId" = CR."Id"),
                       CR."FollowUpStatus",
                       L."Level1"::text,
                       L."Level2"::text,
                       L."Level3"::text,
                       L."Level4"::text,
                       L."Level5"::text,
                       WF."StageId",
                       WF."StageName",
                       WF."ReviewStatus",
                       WF."StageChangedAt",
                       WF."ReviewedAt",
                       WF."ReviewerIds",
                       SV."Severity"
                FROM "CitizenReports" CR
                    INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                    INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                    INNER JOIN "Locations" L ON L."Id" = CR."LocationId"
                    LEFT JOIN LATERAL "GetRecordReviewWorkflow"(electionRoundId, ngoId, 'CitizenReport', CR."Id") WF ON TRUE
                    LEFT JOIN LATERAL "GetRecordSeverity"(electionRoundId, ngoId, 'CitizenReport', CR."Id") SV ON TRUE
                WHERE CR."ElectionRoundId" = electionRoundId
                  AND MN."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                  AND (hasFlaggedAnswers IS NULL
                    OR (CR."NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                    OR (CR."NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE))
                  AND (hasFlaggedAnswers IS NULL OR hasFlaggedAnswers = FALSE OR 1 = 2)
                  AND (followUpStatus IS NULL OR CR."FollowUpStatus" = followUpStatus)
                  AND (level1 IS NULL OR L."Level1" = level1)
                  AND (level2 IS NULL OR L."Level2" = level2)
                  AND (level3 IS NULL OR L."Level3" = level3)
                  AND (level4 IS NULL OR L."Level4" = level4)
                  AND (level5 IS NULL OR L."Level5" = level5)
                  AND (locationId IS NULL OR CR."LocationId" = locationId)
                  AND (formId IS NULL OR CR."FormId" = formId)
                  AND (formStatus IS NULL OR F."Status" = formStatus)
                  AND (questionsAnswered IS NULL
                    OR (questionsAnswered = 'All' AND F."NumberOfQuestions" = CR."NumberOfQuestionsAnswered")
                    OR (questionsAnswered = 'Some' AND F."NumberOfQuestions" <> CR."NumberOfQuestionsAnswered")
                    OR (questionsAnswered = 'None' AND CR."NumberOfQuestionsAnswered" = 0))
                  AND (hasAttachments IS NULL
                    OR ((SELECT COUNT(1) FROM "CitizenReportAttachments" CRA WHERE CRA."CitizenReportId" = CR."Id" AND CRA."IsDeleted" = FALSE AND CRA."IsCompleted" = TRUE) = 0 AND hasAttachments = FALSE)
                    OR ((SELECT COUNT(1) FROM "CitizenReportAttachments" CRA WHERE CRA."CitizenReportId" = CR."Id" AND CRA."IsDeleted" = FALSE AND CRA."IsCompleted" = TRUE) > 0 AND hasAttachments = TRUE))
                  AND (hasNotes IS NULL
                    OR ((SELECT COUNT(1) FROM "CitizenReportNotes" CRN WHERE CRN."CitizenReportId" = CR."Id") = 0 AND hasNotes = FALSE)
                    OR ((SELECT COUNT(1) FROM "CitizenReportNotes" CRN WHERE CRN."CitizenReportId" = CR."Id") > 0 AND hasNotes = TRUE))
                  AND (hasComments IS NULL
                    OR ((SELECT COUNT(1) FROM "CitizenReportComments" CRC INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = CRC."CreatedBy" AND NA."NgoId" = ngoId WHERE CRC."CitizenReportId" = CR."Id") = 0 AND hasComments = FALSE)
                    OR ((SELECT COUNT(1) FROM "CitizenReportComments" CRC INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = CRC."CreatedBy" AND NA."NgoId" = ngoId WHERE CRC."CitizenReportId" = CR."Id") > 0 AND hasComments = TRUE))
                  AND (stageId IS NULL OR WF."StageId" = stageId)
                  AND (reviewStatus IS NULL OR WF."ReviewStatus" = reviewStatus)
                  AND (reviewerId IS NULL OR reviewerId = ANY (WF."ReviewerIds"))
                  AND (hasReviewer IS NULL OR (cardinality(WF."ReviewerIds") > 0) = hasReviewer)
                  AND (severities IS NULL OR cardinality(severities) = 0 OR SV."Severity" = ANY (severities))
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Every citizen report of a round for the platform admins' list, with its form, location and NGO, filtered by the
                -- list's filters (a null filter doesn't filter; fromDate/toDate are timestamps as text, searchText an ILIKE pattern
                -- matched against the form code and the first three location levels). Unordered: the endpoint sorts and pages.
                CREATE OR REPLACE FUNCTION "GetAllCitizenReports"(
                    electionRoundId UUID,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    monitoringNgoId UUID,
                    formId UUID,
                    fromDate TEXT,
                    toDate TEXT,
                    searchText TEXT
                )
                    RETURNS TABLE
                            (
                                "CitizenReportId"           UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormId"                    UUID,
                                "FormCode"                  TEXT,
                                "FormName"                  JSONB,
                                "FormDefaultLanguage"       TEXT,
                                "LocationId"                UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "MonitoringNgoId"           UUID,
                                "NgoName"                   TEXT,
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "FollowUpStatus"            TEXT
                            )
                AS
                $$
                SELECT CR."Id",
                       COALESCE(CR."LastModifiedOn", CR."CreatedOn"),
                       F."Id",
                       F."Code"::text,
                       F."Name",
                       F."DefaultLanguage"::text,
                       L."Id",
                       L."Level1"::text,
                       L."Level2"::text,
                       L."Level3"::text,
                       L."Level4"::text,
                       L."Level5"::text,
                       MN."Id",
                       N."Name"::text,
                       CR."NumberOfQuestionsAnswered",
                       CR."NumberOfFlaggedAnswers",
                       CR."FollowUpStatus"
                FROM "CitizenReports" CR
                    INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                    INNER JOIN "Locations" L ON L."Id" = CR."LocationId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                WHERE CR."ElectionRoundId" = electionRoundId
                  AND (level1 IS NULL OR L."Level1" = level1)
                  AND (level2 IS NULL OR L."Level2" = level2)
                  AND (level3 IS NULL OR L."Level3" = level3)
                  AND (level4 IS NULL OR L."Level4" = level4)
                  AND (level5 IS NULL OR L."Level5" = level5)
                  AND (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                  AND (formId IS NULL OR CR."FormId" = formId)
                  AND (fromDate IS NULL OR COALESCE(CR."LastModifiedOn", CR."CreatedOn") >= fromDate::timestamp)
                  AND (toDate IS NULL OR COALESCE(CR."LastModifiedOn", CR."CreatedOn") <= toDate::timestamp)
                  AND (searchText IS NULL OR F."Code" ILIKE searchText OR L."Level1" ILIKE searchText
                    OR L."Level2" ILIKE searchText OR L."Level3" ILIKE searchText)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The first and last submission times of the citizen reports of the NGO's citizen-reporting round (nulls when there
                -- are none). Always one row.
                CREATE OR REPLACE FUNCTION "GetCitizenReportSubmissionTimestamps"(
                    electionRoundId UUID,
                    ngoId UUID
                )
                    RETURNS TABLE
                            (
                                "FirstSubmissionTimestamp" TIMESTAMP WITH TIME ZONE,
                                "LastSubmissionTimestamp"  TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT MIN(COALESCE(CR."LastModifiedOn", CR."CreatedOn")),
                       MAX(COALESCE(CR."LastModifiedOn", CR."CreatedOn"))
                FROM "CitizenReports" CR
                    INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                    INNER JOIN "MonitoringNgos" MN ON ER."MonitoringNgoForCitizenReportingId" = MN."Id"
                WHERE CR."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The distinct forms of the citizen reports of the NGO's citizen-reporting round, for the list's form filter.
                CREATE OR REPLACE FUNCTION "GetCitizenReportFormFilterOptions"(
                    electionRoundId UUID,
                    ngoId UUID
                )
                    RETURNS TABLE
                            (
                                "FormId"          UUID,
                                "FormName"        JSONB,
                                "FormCode"        TEXT,
                                "DefaultLanguage" TEXT
                            )
                AS
                $$
                SELECT DISTINCT F."Id",
                                F."Name",
                                F."Code"::text,
                                F."DefaultLanguage"::text
                FROM "CitizenReports" CR
                    INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                    INNER JOIN "MonitoringNgos" MN ON ER."MonitoringNgoForCitizenReportingId" = MN."Id"
                    INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                WHERE CR."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The comments on a citizen report of the NGO's citizen-reporting round, with their authors' names. Unordered: the
                -- endpoint sorts by "CreatedAt".
                CREATE OR REPLACE FUNCTION "GetCitizenReportComments"(
                    electionRoundId UUID,
                    ngoId UUID,
                    citizenReportId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"              UUID,
                                "ElectionRoundId" UUID,
                                "CitizenReportId" UUID,
                                "Text"            TEXT,
                                "CreatedBy"       UUID,
                                "CreatedByName"   TEXT,
                                "CreatedAt"       TIMESTAMP WITH TIME ZONE,
                                "LastModifiedAt"  TIMESTAMP WITH TIME ZONE
                            )
                AS
                $$
                SELECT C."Id",
                       C."ElectionRoundId",
                       C."CitizenReportId",
                       C."Text"::text,
                       C."CreatedBy",
                       COALESCE(U."DisplayName", ''),
                       C."CreatedOn",
                       C."LastModifiedOn"
                FROM "CitizenReportComments" C
                    INNER JOIN "CitizenReports" CR ON CR."Id" = C."CitizenReportId"
                    INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = C."CreatedBy"
                WHERE C."ElectionRoundId" = electionRoundId
                  AND C."CitizenReportId" = citizenReportId
                  AND MN."NgoId" = ngoId
                  AND MN."ElectionRoundId" = electionRoundId
                $$ LANGUAGE sql STABLE;
                """);

            // hangfire-exports
            migrationBuilder.Sql(
                """
                -- Returns the comments (AuthorName, Text, Timestamp) of a quick report as a JSONB array, ordered by creation time.
                -- Only comments authored by admins or staff of the given NGO are included.
                CREATE OR REPLACE FUNCTION "ExportGetQuickReportComments"(ngoId UUID, quickReportId UUID)
                RETURNS JSONB AS $$
                    SELECT COALESCE(
                        (
                            SELECT JSONB_AGG(
                                       JSONB_BUILD_OBJECT(
                                           'AuthorName', COALESCE(CMTU."DisplayName", ''),
                                           'Text', CMT."Text",
                                           'Timestamp', COALESCE(CMT."LastModifiedOn", CMT."CreatedOn")
                                       )
                                       ORDER BY CMT."CreatedOn"
                                   )
                            FROM "QuickReportComments" CMT
                                LEFT JOIN "AspNetUsers" CMTU ON CMTU."Id" = CMT."CreatedBy"
                            WHERE CMT."QuickReportId" = quickReportId
                              AND (
                                EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = CMT."CreatedBy" AND NA."NgoId" = ngoId)
                                OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = CMT."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ),
                        '[]'::JSONB
                    );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the comments (AuthorName, Text, Timestamp) of an incident report as a JSONB array, ordered by creation time.
                -- Only comments authored by admins or staff of the given NGO are included.
                CREATE OR REPLACE FUNCTION "ExportGetIncidentReportComments"(ngoId UUID, incidentReportId UUID)
                RETURNS JSONB AS $$
                    SELECT COALESCE(
                        (
                            SELECT JSONB_AGG(
                                       JSONB_BUILD_OBJECT(
                                           'AuthorName', COALESCE(CMTU."DisplayName", ''),
                                           'Text', CMT."Text",
                                           'Timestamp', COALESCE(CMT."LastModifiedOn", CMT."CreatedOn")
                                       )
                                       ORDER BY CMT."CreatedOn"
                                   )
                            FROM "IncidentReportComments" CMT
                                LEFT JOIN "AspNetUsers" CMTU ON CMTU."Id" = CMT."CreatedBy"
                            WHERE CMT."IncidentReportId" = incidentReportId
                              AND (
                                EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = CMT."CreatedBy" AND NA."NgoId" = ngoId)
                                OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = CMT."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ),
                        '[]'::JSONB
                    );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the comments (AuthorName, Text, Timestamp) of a citizen report as a JSONB array, ordered by creation time.
                -- Only comments authored by admins or staff of the given NGO are included.
                CREATE OR REPLACE FUNCTION "ExportGetCitizenReportComments"(ngoId UUID, citizenReportId UUID)
                RETURNS JSONB AS $$
                    SELECT COALESCE(
                        (
                            SELECT JSONB_AGG(
                                       JSONB_BUILD_OBJECT(
                                           'AuthorName', COALESCE(CMTU."DisplayName", ''),
                                           'Text', CMT."Text",
                                           'Timestamp', COALESCE(CMT."LastModifiedOn", CMT."CreatedOn")
                                       )
                                       ORDER BY CMT."CreatedOn"
                                   )
                            FROM "CitizenReportComments" CMT
                                LEFT JOIN "AspNetUsers" CMTU ON CMTU."Id" = CMT."CreatedBy"
                            WHERE CMT."CitizenReportId" = citizenReportId
                              AND (
                                EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = CMT."CreatedBy" AND NA."NgoId" = ngoId)
                                OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = CMT."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ),
                        '[]'::JSONB
                    );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the comments (AuthorName, Text, Timestamp) of a form submission as a JSONB array, ordered by creation time.
                -- Only comments authored by admins or staff of the given NGO are included.
                CREATE OR REPLACE FUNCTION "ExportGetFormSubmissionComments"(ngoId UUID, submissionId UUID)
                RETURNS JSONB AS $$
                    SELECT COALESCE(
                        (
                            SELECT JSONB_AGG(
                                       JSONB_BUILD_OBJECT(
                                           'AuthorName', COALESCE(CMTU."DisplayName", ''),
                                           'Text', CMT."Text",
                                           'Timestamp', COALESCE(CMT."LastModifiedOn", CMT."CreatedOn")
                                       )
                                       ORDER BY CMT."CreatedOn"
                                   )
                            FROM "FormSubmissionComments" CMT
                                LEFT JOIN "AspNetUsers" CMTU ON CMTU."Id" = CMT."CreatedBy"
                            WHERE CMT."SubmissionId" = submissionId
                              AND (
                                EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = CMT."CreatedBy" AND NA."NgoId" = ngoId)
                                OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = CMT."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ),
                        '[]'::JSONB
                    );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the quick reports of an election round visible to an NGO for the quick reports export (with attachments,
                -- comments, observer and polling station details), filtered by the export filters. Unordered.
                CREATE OR REPLACE FUNCTION "ExportGetQuickReports"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    coalitionMemberId UUID,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    followUpStatus TEXT,
                    quickReportLocationType TEXT,
                    incidentCategory TEXT,
                    fromDate TEXT,
                    toDate TEXT,
                    hasAttachments BOOLEAN,
                    monitoringObserverId UUID,
                    pollingStationId UUID,
                    tagsFilter TEXT[]
                )
                RETURNS TABLE (
                    "Id" UUID,
                    "QuickReportLocationType" TEXT,
                    "IncidentCategory" TEXT,
                    "MonitoringObserverId" UUID,
                    "Timestamp" TIMESTAMPTZ,
                    "Title" TEXT,
                    "Description" TEXT,
                    "FollowUpStatus" TEXT,
                    "Attachments" JSONB,
                    "Comments" JSONB,
                    "NgoName" TEXT,
                    "DisplayName" TEXT,
                    "Email" TEXT,
                    "PhoneNumber" TEXT,
                    "Tags" TEXT[],
                    "PollingStationDetails" TEXT,
                    "PollingStationId" UUID,
                    "Level1" TEXT,
                    "Level2" TEXT,
                    "Level3" TEXT,
                    "Level4" TEXT,
                    "Level5" TEXT,
                    "Number" TEXT,
                    "Address" TEXT
                ) AS $$
                    SELECT
                        QR."Id",
                        QR."QuickReportLocationType",
                        QR."IncidentCategory",
                        QR."MonitoringObserverId",
                        QR."LastUpdatedAt" AS "Timestamp",
                        QR."Title"::TEXT,
                        QR."Description"::TEXT,
                        QR."FollowUpStatus",
                        COALESCE(
                            (
                                SELECT
                                    JSONB_AGG(
                                        JSONB_BUILD_OBJECT(
                                            'QuickReportId', "QuickReportId",
                                            'FileName', "FileName",
                                            'MimeType', "MimeType",
                                            'FilePath', "FilePath",
                                            'UploadedFileName', "UploadedFileName",
                                            'TimeSubmitted', QRA."LastUpdatedAt"
                                        )
                                    )
                                FROM "QuickReportAttachments" QRA
                                WHERE QRA."ElectionRoundId" = electionRoundId
                                  AND QRA."QuickReportId" = QR."Id"
                            ),
                            '[]'::JSONB
                        ) AS "Attachments",
                        "ExportGetQuickReportComments"(ngoId, QR."Id") AS "Comments",
                        MO."NgoName"::TEXT,
                        MO."DisplayName",
                        MO."Email",
                        MO."PhoneNumber",
                        MO."Tags",
                        QR."PollingStationDetails"::TEXT,
                        PS."Id" AS "PollingStationId",
                        PS."Level1"::TEXT,
                        PS."Level2"::TEXT,
                        PS."Level3"::TEXT,
                        PS."Level4"::TEXT,
                        PS."Level5"::TEXT,
                        PS."Number"::TEXT,
                        PS."Address"::TEXT
                    FROM "QuickReports" QR
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
                    WHERE (coalitionMemberId IS NULL OR MO."NgoId" = coalitionMemberId)
                      AND (followUpStatus IS NULL OR QR."FollowUpStatus" = followUpStatus)
                      AND (quickReportLocationType IS NULL OR QR."QuickReportLocationType" = quickReportLocationType)
                      AND (incidentCategory IS NULL OR QR."IncidentCategory" = incidentCategory)
                      AND (level1 IS NULL OR PS."Level1" = level1)
                      AND (level2 IS NULL OR PS."Level2" = level2)
                      AND (level3 IS NULL OR PS."Level3" = level3)
                      AND (level4 IS NULL OR PS."Level4" = level4)
                      AND (level5 IS NULL OR PS."Level5" = level5)
                      AND (pollingStationId IS NULL OR QR."PollingStationId" = pollingStationId)
                      AND (fromDate IS NULL OR QR."LastUpdatedAt" >= fromDate::TIMESTAMP)
                      AND (toDate IS NULL OR QR."LastUpdatedAt" <= toDate::TIMESTAMP)
                      AND (
                        hasAttachments IS NULL
                        OR (
                            (
                                SELECT COUNT(1)
                                FROM "QuickReportAttachments" QRA
                                WHERE QRA."QuickReportId" = QR."Id"
                                  AND QRA."IsDeleted" = FALSE
                                  AND QRA."IsCompleted" = TRUE
                            ) = 0
                            AND hasAttachments = FALSE
                        )
                        OR (
                            (
                                SELECT COUNT(1)
                                FROM "QuickReportAttachments" QRA
                                WHERE QRA."QuickReportId" = QR."Id"
                                  AND QRA."IsDeleted" = FALSE
                                  AND QRA."IsCompleted" = TRUE
                            ) > 0
                            AND hasAttachments = TRUE
                        )
                      )
                      AND (monitoringObserverId IS NULL OR MO."MonitoringObserverId" = monitoringObserverId)
                      AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR MO."Tags" && tagsFilter);
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the incident reports of an election round visible to an NGO for the incident reports export (with form,
                -- attachments, notes, comments, observer and polling station details), filtered by the export filters. Unordered.
                CREATE OR REPLACE FUNCTION "ExportGetIncidentReports"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    coalitionMemberId UUID,
                    searchText TEXT,
                    locationType TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    pollingStationId UUID,
                    hasFlaggedAnswers BOOLEAN,
                    followUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    formStatus TEXT,
                    hasNotes BOOLEAN,
                    hasAttachments BOOLEAN,
                    questionsAnswered TEXT,
                    fromDate TEXT,
                    toDate TEXT,
                    isCompleted BOOLEAN
                )
                RETURNS TABLE (
                    "IncidentReportId" UUID,
                    "TimeSubmitted" TIMESTAMPTZ,
                    "FormId" UUID,
                    "FormCode" TEXT,
                    "FormName" JSONB,
                    "FormDefaultLanguage" TEXT,
                    "LocationType" TEXT,
                    "LocationDescription" TEXT,
                    "PollingStationId" UUID,
                    "Level1" TEXT,
                    "Level2" TEXT,
                    "Level3" TEXT,
                    "Level4" TEXT,
                    "Level5" TEXT,
                    "Number" TEXT,
                    "NgoName" TEXT,
                    "MonitoringObserverId" UUID,
                    "DisplayName" TEXT,
                    "Email" TEXT,
                    "PhoneNumber" TEXT,
                    "Tags" TEXT[],
                    "Answers" JSONB,
                    "Attachments" JSONB,
                    "Notes" JSONB,
                    "Comments" JSONB,
                    "FollowUpStatus" TEXT,
                    "NumberOfFlaggedAnswers" INTEGER,
                    "IsCompleted" BOOLEAN
                ) AS $$
                    WITH
                        INCIDENT_REPORTS AS (
                            SELECT
                                IR."Id" AS "IncidentReportId",
                                F."Id" AS "FormId",
                                F."Code" AS "FormCode",
                                F."Name" AS "FormName",
                                F."DefaultLanguage" AS "FormDefaultLanguage",
                                IR."LocationType",
                                IR."LocationDescription",
                                IR."PollingStationId",
                                IR."MonitoringObserverId",
                                IR."NumberOfQuestionsAnswered",
                                IR."NumberOfFlaggedAnswers",
                                IR."Answers",
                                IR."IsCompleted",
                                COALESCE(
                                    (
                                        SELECT
                                            JSONB_AGG(
                                                JSONB_BUILD_OBJECT(
                                                    'QuestionId', "QuestionId",
                                                    'FilePath', "FilePath",
                                                    'UploadedFileName', "UploadedFileName"
                                                )
                                            )
                                        FROM "IncidentReportAttachments" A
                                        WHERE A."ElectionRoundId" = electionRoundId
                                          AND A."FormId" = IR."FormId"
                                          AND A."IncidentReportId" = IR."Id"
                                    ),
                                    '[]'::JSONB
                                ) AS "Attachments",
                                COALESCE(
                                    (
                                        SELECT
                                            JSONB_AGG(JSONB_BUILD_OBJECT('QuestionId', "QuestionId", 'Text', "Text"))
                                        FROM "IncidentReportNotes" N
                                        WHERE N."ElectionRoundId" = electionRoundId
                                          AND N."FormId" = IR."FormId"
                                          AND N."IncidentReportId" = IR."Id"
                                    ),
                                    '[]'::JSONB
                                ) AS "Notes",
                                "ExportGetIncidentReportComments"(ngoId, IR."Id") AS "Comments",
                                IR."LastUpdatedAt" AS "TimeSubmitted",
                                IR."FollowUpStatus",
                                MO."DisplayName",
                                MO."NgoName",
                                MO."PhoneNumber",
                                MO."Email",
                                MO."Tags"
                            FROM "IncidentReports" IR
                                INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                                INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) MO ON MO."MonitoringObserverId" = IR."MonitoringObserverId"
                                INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF ON IR."FormId" = AF."FormId"
                                LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                            WHERE (coalitionMemberId IS NULL OR MO."NgoId" = coalitionMemberId)
                              AND (monitoringObserverStatus IS NULL OR MO."Status" = monitoringObserverStatus)
                              AND (formId IS NULL OR IR."FormId" = formId)
                              AND (formStatus IS NULL OR F."Status" = formStatus)
                              AND (locationType IS NULL OR IR."LocationType" = locationType)
                              AND (questionsAnswered IS NULL
                                OR (questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                                OR (questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                                OR (questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                              )
                              AND (fromDate IS NULL OR IR."LastUpdatedAt" >= fromDate::TIMESTAMP)
                              AND (toDate IS NULL OR IR."LastUpdatedAt" <= toDate::TIMESTAMP)
                              AND (isCompleted IS NULL OR IR."IsCompleted" = isCompleted)
                        )
                    SELECT
                        IR."IncidentReportId",
                        IR."TimeSubmitted",
                        IR."FormId",
                        IR."FormCode"::TEXT,
                        IR."FormName",
                        IR."FormDefaultLanguage"::TEXT,
                        IR."LocationType",
                        IR."LocationDescription"::TEXT,
                        PS."Id" AS "PollingStationId",
                        PS."Level1"::TEXT,
                        PS."Level2"::TEXT,
                        PS."Level3"::TEXT,
                        PS."Level4"::TEXT,
                        PS."Level5"::TEXT,
                        PS."Number"::TEXT,
                        IR."NgoName"::TEXT,
                        IR."MonitoringObserverId",
                        IR."DisplayName",
                        IR."Email",
                        IR."PhoneNumber",
                        IR."Tags",
                        IR."Answers",
                        IR."Attachments",
                        IR."Notes",
                        IR."Comments",
                        IR."FollowUpStatus",
                        IR."NumberOfFlaggedAnswers",
                        IR."IsCompleted"
                    FROM INCIDENT_REPORTS IR
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                    WHERE (
                        searchText IS NULL
                        OR searchText = ''
                        OR IR."DisplayName" ILIKE searchText
                        OR IR."MonitoringObserverId"::TEXT ILIKE searchText
                        OR IR."Email" ILIKE searchText
                        OR IR."PhoneNumber" ILIKE searchText
                      )
                      AND (level1 IS NULL OR PS."Level1" = level1)
                      AND (level2 IS NULL OR PS."Level2" = level2)
                      AND (level3 IS NULL OR PS."Level3" = level3)
                      AND (level4 IS NULL OR PS."Level4" = level4)
                      AND (level5 IS NULL OR PS."Level5" = level5)
                      AND (pollingStationNumber IS NULL OR PS."Number" = pollingStationNumber)
                      AND (pollingStationId IS NULL OR IR."PollingStationId" = pollingStationId)
                      AND (hasFlaggedAnswers IS NULL
                        OR (IR."NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                        OR (IR."NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE)
                      )
                      AND (followUpStatus IS NULL OR IR."FollowUpStatus" = followUpStatus)
                      AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR IR."Tags" && tagsFilter)
                      AND (hasNotes IS NULL
                        OR (JSONB_ARRAY_LENGTH(IR."Notes") = 0 AND hasNotes = FALSE)
                        OR (JSONB_ARRAY_LENGTH(IR."Notes") > 0 AND hasNotes = TRUE)
                      )
                      AND (hasAttachments IS NULL
                        OR (JSONB_ARRAY_LENGTH(IR."Attachments") = 0 AND hasAttachments = FALSE)
                        OR (JSONB_ARRAY_LENGTH(IR."Attachments") > 0 AND hasAttachments = TRUE)
                      );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the citizen reports of an election round for the NGO running citizen reporting, for the citizen reports
                -- export (with form questions, location, attachments, notes and comments), filtered by the export filters. Unordered.
                CREATE OR REPLACE FUNCTION "ExportGetCitizenReports"(
                    electionRoundId UUID,
                    ngoId UUID,
                    hasFlaggedAnswers BOOLEAN,
                    followUpStatus TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    locationId UUID,
                    formId UUID,
                    formStatus TEXT,
                    hasAttachments BOOLEAN,
                    hasNotes BOOLEAN,
                    questionsAnswered TEXT
                )
                RETURNS TABLE (
                    "CitizenReportId" UUID,
                    "FormId" UUID,
                    "Answers" JSONB,
                    "FollowUpStatus" TEXT,
                    "Questions" JSONB,
                    "Level1" TEXT,
                    "Level2" TEXT,
                    "Level3" TEXT,
                    "Level4" TEXT,
                    "Level5" TEXT,
                    "Attachments" JSONB,
                    "Notes" JSONB,
                    "Comments" JSONB,
                    "TimeSubmitted" TIMESTAMPTZ
                ) AS $$
                    SELECT
                        CR."Id" AS "CitizenReportId",
                        CR."FormId",
                        CR."Answers",
                        CR."FollowUpStatus",
                        F."Questions",
                        L."Level1"::TEXT,
                        L."Level2"::TEXT,
                        L."Level3"::TEXT,
                        L."Level4"::TEXT,
                        L."Level5"::TEXT,
                        COALESCE(
                            (
                                SELECT
                                    JSONB_AGG(
                                        JSONB_BUILD_OBJECT(
                                            'QuestionId', "QuestionId",
                                            'FilePath', "FilePath",
                                            'UploadedFileName', "UploadedFileName"
                                        )
                                    )
                                FROM "CitizenReportAttachments" A
                                WHERE A."ElectionRoundId" = electionRoundId
                                  AND A."FormId" = CR."FormId"
                                  AND A."CitizenReportId" = CR."Id"
                            ),
                            '[]'::JSONB
                        ) AS "Attachments",
                        COALESCE(
                            (
                                SELECT
                                    JSONB_AGG(JSONB_BUILD_OBJECT('QuestionId', "QuestionId", 'Text', "Text"))
                                FROM "CitizenReportNotes" N
                                WHERE N."ElectionRoundId" = electionRoundId
                                  AND N."FormId" = CR."FormId"
                                  AND N."CitizenReportId" = CR."Id"
                            ),
                            '[]'::JSONB
                        ) AS "Notes",
                        "ExportGetCitizenReportComments"(ngoId, CR."Id") AS "Comments",
                        COALESCE(CR."LastModifiedOn", CR."CreatedOn") AS "TimeSubmitted"
                    FROM "CitizenReports" CR
                        INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                        INNER JOIN "Locations" L ON L."Id" = CR."LocationId"
                        INNER JOIN "ElectionRounds" E ON CR."ElectionRoundId" = E."Id"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = E."MonitoringNgoForCitizenReportingId"
                    WHERE CR."ElectionRoundId" = electionRoundId
                      AND E."CitizenReportingEnabled" = TRUE
                      AND MN."NgoId" = ngoId
                      AND (hasFlaggedAnswers IS NULL
                        OR (CR."NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                        OR (CR."NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE)
                      )
                      AND (followUpStatus IS NULL OR CR."FollowUpStatus" = followUpStatus)
                      AND (level1 IS NULL OR L."Level1" = level1)
                      AND (level2 IS NULL OR L."Level2" = level2)
                      AND (level3 IS NULL OR L."Level3" = level3)
                      AND (level4 IS NULL OR L."Level4" = level4)
                      AND (level5 IS NULL OR L."Level5" = level5)
                      AND (locationId IS NULL OR CR."LocationId" = locationId)
                      AND (formId IS NULL OR CR."FormId" = formId)
                      AND (formStatus IS NULL OR F."Status" = formStatus)
                      AND (questionsAnswered IS NULL
                        OR (questionsAnswered = 'All' AND F."NumberOfQuestions" = CR."NumberOfQuestionsAnswered")
                        OR (questionsAnswered = 'Some' AND F."NumberOfQuestions" <> CR."NumberOfQuestionsAnswered")
                        OR (questionsAnswered = 'None' AND CR."NumberOfQuestionsAnswered" = 0)
                      )
                      AND (hasAttachments IS NULL
                        OR ((SELECT COUNT(1) FROM "CitizenReportAttachments" WHERE "CitizenReportId" = CR."Id" AND "IsDeleted" = FALSE AND "IsCompleted" = TRUE) = 0 AND hasAttachments = FALSE)
                        OR ((SELECT COUNT(1) FROM "CitizenReportAttachments" WHERE "CitizenReportId" = CR."Id" AND "IsDeleted" = FALSE AND "IsCompleted" = TRUE) > 0 AND hasAttachments = TRUE)
                      )
                      AND (hasNotes IS NULL
                        OR ((SELECT COUNT(1) FROM "CitizenReportNotes" WHERE "CitizenReportId" = CR."Id") = 0 AND hasNotes = FALSE)
                        OR ((SELECT COUNT(1) FROM "CitizenReportNotes" WHERE "CitizenReportId" = CR."Id") > 0 AND hasNotes = TRUE)
                      );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the published-or-not forms (full "Forms" rows) available to an NGO in an election round for the given
                -- data source; used by the form submissions exports to build one sheet per form.
                CREATE OR REPLACE FUNCTION "ExportGetAvailableForms"(electionRoundId UUID, ngoId UUID, dataSource TEXT)
                RETURNS SETOF "Forms" AS $$
                    SELECT F.*
                    FROM "Forms" F
                        INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF ON AF."FormId" = F."Id";
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the polling station information submissions of an election round for the PSI export (polling station,
                -- NGO name and minutes monitoring). Unordered.
                CREATE OR REPLACE FUNCTION "ExportGetPollingStationInformation"(electionRoundId UUID)
                RETURNS TABLE (
                    "SubmissionId" UUID,
                    "PollingStationId" UUID,
                    "Level1" TEXT,
                    "Level2" TEXT,
                    "Level3" TEXT,
                    "Level4" TEXT,
                    "Level5" TEXT,
                    "Number" TEXT,
                    "Address" TEXT,
                    "NgoName" TEXT,
                    "ArrivalTime" TIMESTAMPTZ,
                    "DepartureTime" TIMESTAMPTZ,
                    "MinutesMonitoring" DOUBLE PRECISION,
                    "NumberOfQuestionsAnswered" INTEGER,
                    "NumberOfFlaggedAnswers" INTEGER,
                    "IsCompleted" BOOLEAN,
                    "LastUpdatedAt" TIMESTAMPTZ,
                    "Answers" JSONB
                ) AS $$
                    SELECT
                        PSI."Id" AS "SubmissionId",
                        PS."Id" AS "PollingStationId",
                        PS."Level1"::TEXT,
                        PS."Level2"::TEXT,
                        PS."Level3"::TEXT,
                        PS."Level4"::TEXT,
                        PS."Level5"::TEXT,
                        PS."Number"::TEXT,
                        PS."Address"::TEXT,
                        N."Name"::TEXT AS "NgoName",
                        PSI."ArrivalTime",
                        PSI."DepartureTime",
                        COALESCE("ComputeMinutesMonitoring"(PSI."ArrivalTime", PSI."DepartureTime", PSI."Breaks"), 0) AS "MinutesMonitoring",
                        PSI."NumberOfQuestionsAnswered",
                        PSI."NumberOfFlaggedAnswers",
                        PSI."IsCompleted",
                        PSI."LastUpdatedAt",
                        PSI."Answers"
                    FROM "PollingStationInformation" PSI
                        INNER JOIN "PollingStations" PS ON PS."Id" = PSI."PollingStationId"
                        INNER JOIN "MonitoringObservers" MO ON MO."Id" = PSI."MonitoringObserverId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    WHERE PSI."ElectionRoundId" = electionRoundId;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the polling stations of an election round for the polling stations export (tags default to {}). Unordered.
                CREATE OR REPLACE FUNCTION "ExportGetPollingStations"(electionRoundId UUID)
                RETURNS TABLE (
                    "Id" UUID,
                    "Level1" TEXT,
                    "Level2" TEXT,
                    "Level3" TEXT,
                    "Level4" TEXT,
                    "Level5" TEXT,
                    "Number" TEXT,
                    "Address" TEXT,
                    "DisplayOrder" INTEGER,
                    "Latitude" DOUBLE PRECISION,
                    "Longitude" DOUBLE PRECISION,
                    "Tags" JSONB
                ) AS $$
                    SELECT
                        PS."Id",
                        PS."Level1"::TEXT,
                        PS."Level2"::TEXT,
                        PS."Level3"::TEXT,
                        PS."Level4"::TEXT,
                        PS."Level5"::TEXT,
                        PS."Number"::TEXT,
                        PS."Address"::TEXT,
                        PS."DisplayOrder",
                        PS."Latitude",
                        PS."Longitude",
                        COALESCE(NULLIF(PS."Tags", 'null'::JSONB), '{}'::JSONB) AS "Tags"
                    FROM "PollingStations" PS
                    WHERE PS."ElectionRoundId" = electionRoundId;
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Returns the locations of an election round for the locations export (tags default to {}). Unordered.
                CREATE OR REPLACE FUNCTION "ExportGetLocations"(electionRoundId UUID)
                RETURNS TABLE (
                    "Id" UUID,
                    "Level1" TEXT,
                    "Level2" TEXT,
                    "Level3" TEXT,
                    "Level4" TEXT,
                    "Level5" TEXT,
                    "DisplayOrder" INTEGER,
                    "Tags" JSONB
                ) AS $$
                    SELECT
                        L."Id",
                        L."Level1"::TEXT,
                        L."Level2"::TEXT,
                        L."Level3"::TEXT,
                        L."Level4"::TEXT,
                        L."Level5"::TEXT,
                        L."DisplayOrder",
                        COALESCE(L."Tags", '{}') AS "Tags"
                    FROM "Locations" L
                    WHERE L."ElectionRoundId" = electionRoundId;
                $$ LANGUAGE sql STABLE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewCoalitionHeader"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewMonitoringNgoStatus"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewNgosCounts"(UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewCoalitionsCount"(UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewObservers"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewPollingStationLevels"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewLocationLevels"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewFormsHistogram"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewAnswersHistogram"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewQuickReportsHistogram"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewCitizenReportsHistogram"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewIncidentReportsHistogram"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "StatsOverviewCitizenReportAnswersHistogram"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormStatisticsTotals"(UUID, UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPollingStationStatisticsTotals"(UUID, UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPollingStationStatsOverview"(UUID, UUID, UUID, TEXT, BOOLEAN);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetMonitoringObserverStatsWorkload"(UUID, UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetObserverStatisticsTotals"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetMonitoringObserverPollingStationLevelStats"(UUID, UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetMonitoringObserverStatisticsExtras"(UUID, UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetMonitoringObserverStatisticsAnswersHistogram"(UUID, UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoAdminStatisticsObservers"(UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoAdminStatisticsLevels"(UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoAdminStatisticsFormsHistogram"(UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoAdminStatisticsQuickReportsHistogram"(UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoAdminStatisticsCitizenReportsHistogram"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoAdminStatisticsIncidentReportsHistogram"(UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsCountriesHistogram"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsObservers"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsNgos"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsElectionRounds"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsPollingStations"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsVisitedPollingStations"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsMinutesMonitoring"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsFormSubmissions"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsQuestionsAnswered"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsFlaggedAnswers"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsQuickReports"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsIncidentReports"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminStatisticsCitizenReports"();""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformStatisticsMonitoringObservers"(UUID[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformStatisticsMonitoringNgos"(UUID[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformStatisticsPollingStations"(UUID[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformStatisticsPollingStationVisits"(UUID[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformStatisticsFormSubmissions"(UUID[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformStatisticsQuestionsAnswered"(UUID[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformStatisticsFlaggedAnswers"(UUID[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformStatisticsMinutesMonitoring"(UUID[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetObserverGuideForObserver"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetObserverGuidesForObserver"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoObserverGuides"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetElectionRoundCitizenNotifications"(UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenNotificationsNgoName"(UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoCitizenNotification"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoSentCitizenNotifications"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetObserverReceivedNotifications"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetObserverNotificationsNgoName"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoNotificationDetails"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoSentNotifications"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "CountNgoSentNotifications"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNotificationRecipientTokens"(UUID, UUID, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, BOOLEAN, TEXT, TEXT[], TEXT, UUID, BOOLEAN, BOOLEAN, TEXT, TEXT, TEXT, BOOLEAN, TEXT, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNotificationRecipients"(UUID, UUID, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, BOOLEAN, TEXT, TEXT[], TEXT, UUID, BOOLEAN, BOOLEAN, TEXT, TEXT, TEXT, BOOLEAN, TEXT, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNotificationRecipientObserverIds"(UUID, UUID, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, BOOLEAN, TEXT, TEXT[], TEXT, UUID, BOOLEAN, BOOLEAN, TEXT, TEXT, TEXT, BOOLEAN, TEXT, TEXT, BOOLEAN);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoFormsList"(UUID, UUID, TEXT, TEXT, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoFormDetails"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormTemplatesList"(TEXT, TEXT, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetMonitoringObserversList"(UUID, UUID, TEXT, TEXT[], TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetMonitoringObserverDetails"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetMonitoringObserversExport"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformObserversList"(TEXT, TEXT, BOOLEAN);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformObserverDetails"(UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ObserverHasFormAnswersAtPollingStation"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPollingStationInformationData"(UUID, TEXT, TEXT, TEXT, TEXT, TEXT, UUID, BOOLEAN, TEXT, TEXT, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCorrectableRecords"(UUID, TEXT, UUID, UUID, TEXT, TEXT, BOOLEAN, TEXT, TEXT, TEXT, TEXT, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetObserverPollingStationVisits"(UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetAvailableNgosForElectionRound"(UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetElectionRoundOverviews"(TEXT, TEXT, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetElectionRoundCoalitions"(UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoDetails"(UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoOverviews"(TEXT, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionComments"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionsAvailableForms"(UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "HasObserverFormSubmissionsAtPollingStation"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetElectionRoundFormSubmissionRows"(UUID, TEXT, TEXT, TEXT, TEXT, TEXT, UUID, UUID, BOOLEAN, TEXT, TEXT, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionViewV2"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionView"(UUID, UUID, UUID);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionObserverOverviews"(UUID, UUID, TEXT, UUID, TEXT, TEXT[], TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionsByForm"(UUID, UUID, TEXT, UUID, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT, BOOLEAN, TEXT, TEXT[], TEXT, UUID, BOOLEAN, BOOLEAN, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionFilterObservers"(UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionFilterForms"(UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionFilterTimestamps"(UUID, UUID, TEXT);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportComments"(uuid, uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportsObserverFilterOptions"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportsTimestampFilterOptions"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetPlatformAdminQuickReports"(uuid, text, text, text, text, text, uuid, text, text, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetObserverQuickReport"(uuid, uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetNgoQuickReportDetails"(uuid, uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportOverviews"(uuid, uuid, text, uuid, uuid, text[], text, uuid, text, uuid, boolean, text[], text, text, text, text, text, text, text, text, uuid, text, text, boolean, boolean);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportComments"(uuid, uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportObserversOverview"(uuid, uuid, text, text, text[], text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportObserverFilterOptions"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportFormFilterOptions"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportTimestampsFilterOptions"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportsForPlatformAdmin"(uuid, text, text, text, text, text, uuid, uuid, boolean, text, text, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportListEntries"(uuid, uuid, uuid, text, text, text, text, text, text, text, uuid, boolean, text, text[], text, uuid, text, text, text, boolean, boolean, boolean, text, text, uuid, text, uuid, boolean, text[], text, integer, integer);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportListEntriesCount"(uuid, uuid, uuid, text, text, text, text, text, text, text, uuid, boolean, text, text[], text, uuid, text, text, text, boolean, boolean, boolean, text, text, uuid, text, uuid, boolean, text[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportComments"(uuid, uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportFormFilterOptions"(uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportSubmissionTimestamps"(uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetAllCitizenReports"(uuid, text, text, text, text, text, uuid, uuid, text, text, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportListEntries"(uuid, uuid, text, text, text, text, text, text, uuid, uuid, text, text, boolean, boolean, boolean, boolean, uuid, text, uuid, boolean, text[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetLocations"(uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetPollingStations"(uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetPollingStationInformation"(uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetAvailableForms"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetCitizenReports"(uuid, uuid, boolean, text, text, text, text, text, text, uuid, uuid, text, boolean, boolean, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetIncidentReports"(uuid, uuid, text, uuid, text, text, text, text, text, text, text, text, uuid, boolean, text, text[], text, uuid, text, boolean, boolean, text, text, text, boolean);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetQuickReports"(uuid, uuid, text, uuid, text, text, text, text, text, text, text, text, text, text, boolean, uuid, uuid, text[]);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetFormSubmissionComments"(uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetCitizenReportComments"(uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetIncidentReportComments"(uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExportGetQuickReportComments"(uuid, uuid);""");
        }
    }
}
