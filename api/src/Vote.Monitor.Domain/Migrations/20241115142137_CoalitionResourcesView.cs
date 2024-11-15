using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CoalitionResourcesView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            """
            CREATE OR REPLACE FUNCTION "GetMonitoringNgoDetails"(
              electionRoundId UUID,
              ngoId UUID
            ) RETURNS TABLE (
              "MonitoringNgoId" UUID,
              "CoalitionId" UUID, 
              "IsCoalitionLeader" BOOLEAN
            ) AS $$ BEGIN RETURN QUERY 
            SELECT 
              MN."Id" AS "MonitoringNgoId", 
              -- Get coalition ID
              (
                SELECT 
                  C."Id" 
                FROM 
                  "CoalitionMemberships" CM 
                  JOIN "Coalitions" C ON CM."CoalitionId" = C."Id" 
                WHERE 
                  CM."MonitoringNgoId" = MN."Id" 
                  AND CM."ElectionRoundId" = MN."ElectionRoundId" 
                LIMIT 
                  1
              ) AS "CoalitionId", 
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
              ) AS "IsCoalitionLeader" 
            FROM 
              "MonitoringNgos" MN 
            WHERE 
              MN."ElectionRoundId" = electionRoundId 
              AND MN."NgoId" = ngoId 
            LIMIT 
              1;
            END;
            $$ LANGUAGE plpgsql;
            """);
            
            migrationBuilder.Sql(
            """
            CREATE OR REPLACE FUNCTION "GetAvailableMonitoringObservers"(
              electionRoundId UUID, 
              ngoId UUID, 
              dataSource TEXT
            ) RETURNS TABLE (
              "MonitoringObserverId" UUID, 
              "DisplayName" TEXT,
              "Email" TEXT, 
              "PhoneNumber" TEXT,
              "Tags" TEXT[],
              "Status" TEXT
            ) AS $$ BEGIN RETURN QUERY 
            SELECT 
              MO."Id" as "MonitoringObserverId", 
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
              MO."Status" AS "Status" 
            FROM 
              "Coalitions" C 
              INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND ON MND."CoalitionId" = C."Id" 
              INNER JOIN "CoalitionMemberships" CM ON C."Id" = CM."CoalitionId" 
              INNER JOIN "MonitoringObservers" MO ON MO."MonitoringNgoId" = CM."MonitoringNgoId" 
              INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId" 
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
              U."DisplayName" AS "DisplayName", 
              U."Email"::TEXT AS "Email", 
              U."PhoneNumber" AS "PhoneNumber", 
              MO."Tags" AS "Tags", 
              MO."Status" AS "Status" 
            FROM 
              "MonitoringObservers" MO 
              INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId" 
              INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND ON MND."MonitoringNgoId" = MO."MonitoringNgoId" 
              INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId" 
            WHERE 
              MND."CoalitionId" IS NULL 
              AND MN."NgoId" = ngoId;
            END;
            $$ LANGUAGE plpgsql;
            
            """);
            
            migrationBuilder.Sql(
            """
            CREATE OR REPLACE FUNCTION "GetAvailableForms"(
                electionRoundId UUID,
                ngoId UUID,
                dataSource TEXT
            )
            RETURNS TABLE (
                "FormId" UUID
            ) AS $$
            BEGIN
                RETURN QUERY
                SELECT F."Id" as "FormId"
                FROM "CoalitionFormAccess" CFA
                INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND ON MND."CoalitionId" = C."Id"
                INNER JOIN "MonitoringNgos" MN ON MND."MonitoringNgoId" = MN."Id"
                INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                WHERE 
                    MND."CoalitionId" IS NOT NULL
                    AND (
                        (
                            (dataSource = 'Ngo' OR dataSource = 'Coalition')
                            AND NOT EXISTS (SELECT 1 FROM "GetMonitoringNgoDetails"(electionRoundId, ngoId))
                            AND MN."NgoId" = ngoId
                        )
                        OR (
                            dataSource = 'Coalition'
                            AND EXISTS (SELECT 1 FROM "GetMonitoringNgoDetails"(electionRoundId, ngoId) WHERE "IsCoalitionLeader")
                        )
                        OR (
                            dataSource = 'Ngo'
                            AND EXISTS (SELECT 1 FROM "GetMonitoringNgoDetails"(electionRoundId, ngoId) WHERE "IsCoalitionLeader")
                            AND MN."NgoId" = ngoId
                        )
                        OR MN."NgoId" = ngoId
                    )
                UNION
                SELECT F."Id" AS "FormId"
                FROM "Forms" F
                  INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND ON MND."MonitoringNgoId" = F."MonitoringNgoId" 
                  INNER JOIN "MonitoringNgos" MN ON MN."Id" = MND."MonitoringNgoId" 
                WHERE 
                  MND."CoalitionId" IS NULL 
                  AND F."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId;
            END;
            $$ LANGUAGE plpgsql;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ""GetAvailableForms"";");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ""GetAvailableMonitoringObservers"";");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ""GetMonitoringNgoDetails"";");
        }
    }
}
