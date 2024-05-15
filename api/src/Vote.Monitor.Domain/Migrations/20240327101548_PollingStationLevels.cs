using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class PollingStationLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level1",
                table: "PollingStations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Level2",
                table: "PollingStations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Level3",
                table: "PollingStations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Level4",
                table: "PollingStations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Level5",
                table: "PollingStations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "PollingStations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level1",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "Level2",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "Level3",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "Level4",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "Level5",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "PollingStations");
        }
    }
}
