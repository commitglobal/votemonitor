using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGetAvailableMonitoringObservers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ""GetAvailableMonitoringObservers"";");
            migrationBuilder.Sql("""
            CREATE OR REPLACE FUNCTION "GetAvailableMonitoringObservers"(
                electionRoundId UUID,
                ngoId UUID,
                dataSource TEXT
            )
                RETURNS TABLE
                        (
                            "MonitoringObserverId" UUID,
                            "NgoId"                UUID,
                            "MonitoringNgoId"      UUID,
                            "DisplayName"          TEXT,
                            "FirstName"            TEXT,
                            "LastName"             TEXT,
                            "Email"                TEXT,
                            "PhoneNumber"          TEXT,
                            "Tags"                 TEXT[],
                            "Status"               TEXT,
                            "AccountStatus"        TEXT,
                            "NgoName"              varchar(256),
                            "IsOwnObserver"        BOOLEAN
                        )
            AS
            $$
            BEGIN
                RETURN QUERY
                    SELECT MO."Id"                      as "MonitoringObserverId",
                           MN."NgoId",
                           MO."MonitoringNgoId",
                           CASE
                               WHEN (
                                   (SELECT "IsCoalitionLeader"
                                    FROM
                                        "GetMonitoringNgoDetails"(electionRoundId, ngoId))
                                       AND MN."NgoId" <> ngoId
                                   ) THEN MO."Id"::TEXT
                               ELSE U."DisplayName" END AS "DisplayName",
                           CASE
                               WHEN (
                                   (SELECT "IsCoalitionLeader"
                                    FROM
                                        "GetMonitoringNgoDetails"(electionRoundId, ngoId))
                                       AND MN."NgoId" <> ngoId
                                   ) THEN MO."Id"::TEXT
                               ELSE U."FirstName" END   AS "FirstName",
                           CASE
                               WHEN (
                                   (SELECT "IsCoalitionLeader"
                                    FROM
                                        "GetMonitoringNgoDetails"(electionRoundId, ngoId))
                                       AND MN."NgoId" <> ngoId
                                   ) THEN MO."Id"::TEXT
                               ELSE U."LastName" END    AS "LastName",
                           CASE
                               WHEN (
                                   (SELECT "IsCoalitionLeader"
                                    FROM "GetMonitoringNgoDetails"(electionRoundId, ngoId))
                                       AND MN."NgoId" <> ngoId
                                   ) THEN MO."Id"::TEXT
                               ELSE U."Email"::TEXT END AS "Email",
                           CASE
                               WHEN (
                                   (SELECT "IsCoalitionLeader"
                                    FROM
                                        "GetMonitoringNgoDetails"(electionRoundId, ngoId))
                                       AND MN."NgoId" <> ngoId
                                   ) THEN MO."Id"::TEXT
                               ELSE U."PhoneNumber" END AS "PhoneNumber",
                           CASE
                               WHEN (
                                   (SELECT "IsCoalitionLeader"
                                    FROM
                                        "GetMonitoringNgoDetails"(electionRoundId, ngoId))
                                       AND MN."NgoId" <> ngoId
                                   ) THEN '{}'::TEXT[]
                               ELSE MO."Tags" END       AS "Tags",
                           MO."Status"                  AS "Status",
                           U."Status"                   AS "AccountStatus",
                           N."Name"                     as "NgoName",
                           CASE
                               WHEN mn."NgoId" = ngoId THEN true
                               ELSE false
                               END                      AS "IsOwnObserver"
                    FROM "Coalitions" C
                             INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND ON MND."CoalitionId" = C."Id"
                             INNER JOIN "CoalitionMemberships" CM ON C."Id" = CM."CoalitionId"
                             INNER JOIN "MonitoringObservers" MO ON MO."MonitoringNgoId" = CM."MonitoringNgoId"
                             INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                             INNER JOIN "Ngos" N on MN."NgoId" = N."Id"
                             INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                    WHERE MND."CoalitionId" IS NOT NULL
                      AND (
                        (
                            (
                                dataSource = 'Ngo'
                                    OR dataSource = 'Coalition'
                                )
                                AND NOT EXISTS (SELECT 1
                                                FROM
                                                    "GetMonitoringNgoDetails"(electionRoundId, ngoId))
                                AND MN."NgoId" = ngoId
                            )
                            OR (
                            dataSource = 'Coalition'
                                AND EXISTS (SELECT 1
                                            FROM
                                                "GetMonitoringNgoDetails"(electionRoundId, ngoId)
                                            WHERE "IsCoalitionLeader")
                            )
                            OR (
                            dataSource = 'Ngo'
                                AND EXISTS (SELECT 1
                                            FROM
                                                "GetMonitoringNgoDetails"(electionRoundId, ngoId)
                                            WHERE "IsCoalitionLeader")
                                AND MN."NgoId" = ngoId
                            )
                            OR MN."NgoId" = ngoId
                        )
                    UNION
                    SELECT MO."Id"         as "MonitoringObserverId",
                           MN."NgoId",
                           MO."MonitoringNgoId",
                           U."DisplayName" AS "DisplayName",
                           U."FirstName"   AS "FirstName",
                           U."LastName"    AS "LastName",
                           U."Email"::TEXT AS "Email",
                           U."PhoneNumber" AS "PhoneNumber",
                           MO."Tags"       AS "Tags",
                           MO."Status"     AS "Status",
                           U."Status"      AS "AccountStatus",
                           N."Name"        as "NgoName",
                           TRUE            as "IsOwnObserver"
                    FROM "MonitoringObservers" MO
                             INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                             INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND
                                        ON MND."MonitoringNgoId" = MO."MonitoringNgoId"
                             INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                             INNER JOIN "Ngos" N on MN."NgoId" = N."Id"
                    WHERE (MND."CoalitionId" IS NULL or MND."IsCoalitionLeader" = false)
                      AND MN."NgoId" = ngoId;
            END;
            $$ LANGUAGE plpgsql;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ""GetAvailableMonitoringObservers"";");
             migrationBuilder.Sql(
            """
            CREATE OR REPLACE FUNCTION "GetAvailableMonitoringObservers"(
              electionRoundId UUID, 
              ngoId UUID, 
              dataSource TEXT
            ) RETURNS TABLE (
              "MonitoringObserverId" UUID, 
              "NgoId" UUID,
              "MonitoringNgoId" UUID,
              "DisplayName" TEXT,
              "Email" TEXT, 
              "PhoneNumber" TEXT,
              "Tags" TEXT[],
              "Status" TEXT,
              "AccountStatus" TEXT,
              "NgoName" varchar(256),
              "IsOwnObserver" BOOLEAN
            ) AS $$ BEGIN RETURN QUERY 
            SELECT 
              MO."Id" as "MonitoringObserverId",
              MN."NgoId",
              MO."MonitoringNgoId",
              CASE WHEN (
                (
                  SELECT 
                    "IsCoalitionLeader" 
                  FROM 
                    "GetMonitoringNgoDetails"(electionRoundId, ngoId)
                ) 
                AND MN."NgoId" <> ngoId
              ) THEN MO."Id"::TEXT ELSE U."DisplayName" END AS "DisplayName", 
              CASE WHEN (
                (
                  SELECT 
                    "IsCoalitionLeader" 
                  FROM "GetMonitoringNgoDetails"(electionRoundId, ngoId)
                ) 
                AND MN."NgoId" <> ngoId
              ) THEN MO."Id"::TEXT ELSE U."Email"::TEXT END AS "Email", 
              CASE WHEN (
                (
                  SELECT 
                    "IsCoalitionLeader" 
                  FROM 
                    "GetMonitoringNgoDetails"(electionRoundId, ngoId)
                ) 
                AND MN."NgoId" <> ngoId
              ) THEN MO."Id"::TEXT ELSE U."PhoneNumber" END AS "PhoneNumber", 
              CASE WHEN (
                (
                  SELECT 
                    "IsCoalitionLeader" 
                  FROM 
                    "GetMonitoringNgoDetails"(electionRoundId, ngoId)
                ) 
                AND MN."NgoId" <> ngoId
              ) THEN '{}'::TEXT[] ELSE MO."Tags" END AS "Tags", 
              MO."Status" AS "Status",
              U."Status" AS "AccountStatus",
              N."Name" as "NgoName",
              CASE 
                   WHEN mn."NgoId" = ngoId THEN true
                   ELSE false
              END AS "IsOwnObserver"
            FROM 
              "Coalitions" C 
              INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND ON MND."CoalitionId" = C."Id" 
              INNER JOIN "CoalitionMemberships" CM ON C."Id" = CM."CoalitionId" 
              INNER JOIN "MonitoringObservers" MO ON MO."MonitoringNgoId" = CM."MonitoringNgoId" 
              INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId" 
              INNER JOIN "Ngos" N on MN."NgoId" = N."Id"
              INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId" 
            WHERE 
              MND."CoalitionId" IS NOT NULL 
              AND (
                (
                  (
                    dataSource = 'Ngo' 
                    OR dataSource = 'Coalition'
                  ) 
                  AND NOT EXISTS (
                    SELECT 
                      1 
                    FROM 
                      "GetMonitoringNgoDetails"(electionRoundId, ngoId)
                  ) 
                  AND MN."NgoId" = ngoId
                ) 
                OR (
                  dataSource = 'Coalition' 
                  AND EXISTS (
                    SELECT 
                      1 
                    FROM 
                      "GetMonitoringNgoDetails"(electionRoundId, ngoId) 
                    WHERE 
                      "IsCoalitionLeader"
                  )
                ) 
                OR (
                  dataSource = 'Ngo' 
                  AND EXISTS (
                    SELECT 
                      1 
                    FROM 
                      "GetMonitoringNgoDetails"(electionRoundId, ngoId) 
                    WHERE 
                      "IsCoalitionLeader"
                  ) 
                  AND MN."NgoId" = ngoId
                ) 
                OR MN."NgoId" = ngoId
              ) 
            UNION 
            SELECT 
              MO."Id" as "MonitoringObserverId", 
              MN."NgoId",
              MO."MonitoringNgoId",
              U."DisplayName" AS "DisplayName", 
              U."Email"::TEXT AS "Email", 
              U."PhoneNumber" AS "PhoneNumber", 
              MO."Tags" AS "Tags", 
              MO."Status" AS "Status",
              U."Status" AS "AccountStatus",
              N."Name" as "NgoName",
              TRUE as "IsOwnObserver"
            FROM 
              "MonitoringObservers" MO 
              INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId" 
              INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND ON MND."MonitoringNgoId" = MO."MonitoringNgoId" 
              INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId" 
              INNER JOIN "Ngos" N on MN."NgoId" = N."Id"
            WHERE 
            (MND."CoalitionId" IS NULL or MND."IsCoalitionLeader" = false) AND MN."NgoId" = ngoId;
            END;
            $$ LANGUAGE plpgsql;
            """);
            

        }
    }
}
