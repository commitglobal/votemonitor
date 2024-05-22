using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowUpStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedsFollowUp",
                table: "FormSubmissions");

            migrationBuilder.AddColumn<string>(
                name: "FollowUpStatus",
                table: "QuickReports",
                type: "text",
                nullable: false,
                defaultValue: "NotApplicable");

            migrationBuilder.AddColumn<string>(
                name: "FollowUpStatus",
                table: "FormSubmissions",
                type: "text",
                nullable: false,
                defaultValue: "NotApplicable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowUpStatus",
                table: "QuickReports");

            migrationBuilder.DropColumn(
                name: "FollowUpStatus",
                table: "FormSubmissions");

            migrationBuilder.AddColumn<bool>(
                name: "NeedsFollowUp",
                table: "FormSubmissions",
                type: "boolean",
                nullable: true);
        }
    }
}
