using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableEntityColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "PollingStationNote");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "PollingStationAttachment");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "NotificationToken");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Notification");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "PollingStationNote",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "PollingStationNote",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "PollingStationNote",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "PollingStationAttachment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "PollingStationAttachment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "PollingStationAttachment",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "NotificationToken",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "NotificationToken",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "NotificationToken",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Notification",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Notification",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Notification",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PollingStationNote");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PollingStationNote");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "PollingStationNote");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PollingStationAttachment");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PollingStationAttachment");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "PollingStationAttachment");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "NotificationToken");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "NotificationToken");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "NotificationToken");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Notification");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "PollingStationNote",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "PollingStationAttachment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "NotificationToken",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "Notification",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
