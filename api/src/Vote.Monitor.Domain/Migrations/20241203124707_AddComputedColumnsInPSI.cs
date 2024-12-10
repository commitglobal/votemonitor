using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddComputedColumnsInPSI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MinutesMonitoring",
                table: "PollingStationInformation",
                type: "double precision",
                nullable: true,
                computedColumnSql: "GREATEST(\n                EXTRACT(EPOCH FROM (\"DepartureTime\" - \"ArrivalTime\")) / 60 \n                - \"ComputeBreaksDuration\"(\"Breaks\"), \n                0\n            )",
                stored: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BreaksDurationInMinutes",
                table: "PollingStationInformation",
                type: "double precision",
                nullable: true,
                computedColumnSql: "\"ComputeBreaksDuration\"(\"Breaks\")",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreaksDurationInMinutes",
                table: "PollingStationInformation");

            migrationBuilder.AlterColumn<double>(
                name: "MinutesMonitoring",
                table: "PollingStationInformation",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true,
                oldComputedColumnSql: "GREATEST(\n                EXTRACT(EPOCH FROM (\"DepartureTime\" - \"ArrivalTime\")) / 60 \n                - \"ComputeBreaksDuration\"(\"Breaks\"), \n                0\n            )");
        }
    }
}
