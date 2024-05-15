using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ReworkPSI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfQuestions",
                table: "PollingStationInformationForms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "MinutesMonitoring",
                table: "PollingStationInformation",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfAnswers",
                table: "PollingStationInformation",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfQuestions",
                table: "PollingStationInformationForms");

            migrationBuilder.DropColumn(
                name: "MinutesMonitoring",
                table: "PollingStationInformation");

            migrationBuilder.DropColumn(
                name: "NumberOfAnswers",
                table: "PollingStationInformation");
        }
    }
}
