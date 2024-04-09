using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FormTweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FormStationsVersion",
                table: "MonitoringNgos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string[]>(
                name: "Languages",
                table: "FormTemplates",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormStationsVersion",
                table: "MonitoringNgos");

            migrationBuilder.AlterColumn<string>(
                name: "Languages",
                table: "FormTemplates",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }
    }
}
