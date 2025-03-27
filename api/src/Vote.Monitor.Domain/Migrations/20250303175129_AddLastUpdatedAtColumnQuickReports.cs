using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUpdatedAtColumnQuickReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "QuickReportAttachments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "QuickReportAttachments");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "QuickReportAttachments");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "QuickReportAttachments",
                newName: "LastUpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdatedAt",
                table: "QuickReportAttachments",
                newName: "CreatedOn");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "QuickReportAttachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "QuickReportAttachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "QuickReportAttachments",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
