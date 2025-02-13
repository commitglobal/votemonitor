using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFormTemplatesIconColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "FormTemplates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LanguagesTranslationStatus",
                table: "FormTemplates",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "FormTemplates");

            migrationBuilder.DropColumn(
                name: "LanguagesTranslationStatus",
                table: "FormTemplates");
        }
    }
}
