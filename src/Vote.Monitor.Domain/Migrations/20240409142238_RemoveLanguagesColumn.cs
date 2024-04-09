using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLanguagesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Languages",
                table: "PollingStationInformationForms");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "FormTemplates");

            migrationBuilder.AddColumn<Guid>(
                name: "FormStationsVersion",
                table: "MonitoringNgos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormStationsVersion",
                table: "MonitoringNgos");

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "PollingStationInformationForms",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "FormTemplates",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
