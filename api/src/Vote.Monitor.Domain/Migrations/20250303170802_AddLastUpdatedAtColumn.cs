using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUpdatedAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "QuickReports");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "QuickReports");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "QuickReports");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PollingStationInformation");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PollingStationInformation");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "PollingStationInformation");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "QuickReports",
                newName: "LastUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "PollingStationInformation",
                newName: "LastUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Notes",
                newName: "LastUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "IncidentReports",
                newName: "LastUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "FormSubmissions",
                newName: "LastUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Attachments",
                newName: "LastUpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdatedAt",
                table: "QuickReports",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAt",
                table: "PollingStationInformation",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAt",
                table: "Notes",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAt",
                table: "IncidentReports",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAt",
                table: "FormSubmissions",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAt",
                table: "Attachments",
                newName: "CreatedOn");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "QuickReports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "QuickReports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "QuickReports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "PollingStationInformation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "PollingStationInformation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "PollingStationInformation",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Notes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Notes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Notes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "IncidentReports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "IncidentReports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "IncidentReports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "FormSubmissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "FormSubmissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "FormSubmissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Attachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Attachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Attachments",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
