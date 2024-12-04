using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveComputedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreaksDurationInMinutes",
                table: "PollingStationInformation");

            migrationBuilder.DropColumn(
                name: "MinutesMonitoring",
                table: "PollingStationInformation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BreaksDurationInMinutes",
                table: "PollingStationInformation",
                type: "double precision",
                nullable: true,
                computedColumnSql: "\"ComputeBreaksDuration\"(\"Breaks\")",
                stored: true);

            migrationBuilder.AddColumn<double>(
                name: "MinutesMonitoring",
                table: "PollingStationInformation",
                type: "double precision",
                nullable: true,
                computedColumnSql: "GREATEST(\n                EXTRACT(EPOCH FROM (\"DepartureTime\" - \"ArrivalTime\")) / 60 \n                - \"ComputeBreaksDuration\"(\"Breaks\"), \n                0\n            )",
                stored: true);
        }
    }
}
