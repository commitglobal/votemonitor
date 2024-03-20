using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentsFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Filename",
                table: "PollingStationAttachment",
                newName: "FileName");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "PollingStationAttachment",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "PollingStationAttachment");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "PollingStationAttachment",
                newName: "Filename");
        }
    }
}
