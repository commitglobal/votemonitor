using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class QuickReportNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IssueType",
                table: "QuickReports",
                type: "text",
                nullable: false,
                defaultValue: "A");

            migrationBuilder.AddColumn<string>(
                name: "OfficialComplaintFilingStatus",
                table: "QuickReports",
                type: "text",
                nullable: false,
                defaultValue: "DoesNotApplyOrOther");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueType",
                table: "QuickReports");

            migrationBuilder.DropColumn(
                name: "OfficialComplaintFilingStatus",
                table: "QuickReports");
        }
    }
}
