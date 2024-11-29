using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddGetAvailableGuides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            """
            CREATE OR REPLACE FUNCTION "GetAvailableGuides"(
                electionRoundId UUID,
                ngoId UUID,
                dataSource TEXT
            )
                RETURNS TABLE
                        (
                            "GuideId" UUID
                        )
            AS
            $$
            BEGIN
                RETURN QUERY
                    SELECT G."Id" as "GuideId"
                    FROM "CoalitionGuideAccess" CGA
                             INNER JOIN "Coalitions" C ON CGA."CoalitionId" = C."Id"
                             INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND ON MND."CoalitionId" = C."Id"
                             INNER JOIN "MonitoringNgos" MN ON MND."MonitoringNgoId" = MN."Id"
                             INNER JOIN "ObserversGuides" G ON CGA."GuideId" = G."Id"
                    WHERE MND."CoalitionId" IS NOT NULL
                      AND g."IsDeleted" = FALSE
                      AND (
                        (
                            dataSource = 'Coalition'
                                AND
                            EXISTS (SELECT 1 FROM "GetMonitoringNgoDetails"(electionRoundId, ngoId) WHERE "IsCoalitionLeader")
                            )
                            OR (
                            dataSource = 'Ngo'
                                --AND EXISTS (SELECT 1 FROM "GetMonitoringNgoDetails"(electionRoundId, ngoId) WHERE "IsCoalitionLeader")
                                AND MN."NgoId" = ngoId AND mn."Id" = CGA."MonitoringNgoId"
                            )
                            OR MN."NgoId" = ngoId AND mn."Id" = CGA."MonitoringNgoId"
                        )
                    UNION
                    SELECT G."Id" as "GuideId"
                    FROM "ObserversGuides" G
                             INNER JOIN "GetMonitoringNgoDetails"(electionRoundId, ngoId) MND
                                        ON MND."MonitoringNgoId" = G."MonitoringNgoId"
                             INNER JOIN "MonitoringNgos" MN ON MN."Id" = MND."MonitoringNgoId"
                    WHERE (MND."CoalitionId" IS NULL or MND."IsCoalitionLeader" = false)
                      AND g."IsDeleted" = FALSE
                      AND MN."NgoId" = ngoId
                      AND MN."ElectionRoundId" = electionRoundId;
            END;
            $$ LANGUAGE plpgsql;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ""GetAvailableGuides"";");
        }
    }
}
