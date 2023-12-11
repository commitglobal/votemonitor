using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FormsQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "Forms");

            migrationBuilder.AddColumn<Guid>(
                name: "LanguageId",
                table: "Forms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Questions",
                table: "Forms",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "Questions",
                table: "Forms");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "Forms",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
