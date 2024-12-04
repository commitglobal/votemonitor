using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddMinutesMonitoringFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "ComputeMinutesMonitoring"(
                    arrivalTime timestamp with time zone,
                    DepartureTime timestamp with time zone,
                    Breaks jsonb
                )
                RETURNS double precision
                LANGUAGE plpgsql
                IMMUTABLE
                AS $$
                BEGIN
                    return GREATEST(EXTRACT(EPOCH FROM (DepartureTime - ArrivalTime)) / 60 - "ComputeBreaksDuration"(Breaks), 0);
                END;
                $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ""ComputeMinutesMonitoring"";");
        }
    }
}
