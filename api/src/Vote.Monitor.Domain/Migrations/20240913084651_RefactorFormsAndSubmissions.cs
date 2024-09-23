using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RefactorFormsAndSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "PollingStationInformationForms",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PollingStationInformationForms",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FormType",
                table: "PollingStationInformationForms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LanguagesTranslationStatus",
                table: "PollingStationInformationForms",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PollingStationInformationForms",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PollingStationInformationForms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfFlaggedAnswers",
                table: "PollingStationInformation",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "PollingStationInformationForms");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PollingStationInformationForms");

            migrationBuilder.DropColumn(
                name: "FormType",
                table: "PollingStationInformationForms");

            migrationBuilder.DropColumn(
                name: "LanguagesTranslationStatus",
                table: "PollingStationInformationForms");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PollingStationInformationForms");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PollingStationInformationForms");

            migrationBuilder.DropColumn(
                name: "NumberOfFlaggedAnswers",
                table: "PollingStationInformation");
        }
    }
}
