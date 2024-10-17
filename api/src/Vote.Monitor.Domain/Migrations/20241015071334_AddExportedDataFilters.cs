using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddExportedDataFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CitizenReportsFilers",
                table: "ExportedData",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormSubmissionsFilters",
                table: "ExportedData",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncidentReportsFilters",
                table: "ExportedData",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuickReportsFilters",
                table: "ExportedData",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CitizenReportsFilers",
                table: "ExportedData");

            migrationBuilder.DropColumn(
                name: "FormSubmissionsFilters",
                table: "ExportedData");

            migrationBuilder.DropColumn(
                name: "IncidentReportsFilters",
                table: "ExportedData");

            migrationBuilder.DropColumn(
                name: "QuickReportsFilters",
                table: "ExportedData");
        }
    }
}
