using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddComputeMinutesMonitoringFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "ComputeBreaksDuration"(
                    breaks jsonb
                )
                RETURNS double precision
                LANGUAGE plpgsql
                IMMUTABLE
                AS $$
                DECLARE
                    total_duration double precision;
                BEGIN
                    SELECT COALESCE(SUM(EXTRACT(EPOCH FROM (("Break" ->> 'End')::timestamp - ("Break" ->> 'Start')::timestamp)) / 60), 0)
                    INTO total_duration
                    FROM jsonb_array_elements(breaks) AS "Break";
                
                    RETURN total_duration;
                END;
                $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ""ComputeBreaksDuration"";");
        }
    }
}
