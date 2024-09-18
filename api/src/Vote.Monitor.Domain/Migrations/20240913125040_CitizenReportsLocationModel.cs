using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CitizenReportsLocationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "CitizenReports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level1 = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Level2 = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Level3 = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Level4 = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Level5 = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitizenReports_LocationId",
                table: "CitizenReports",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ElectionRoundId",
                table: "Locations",
                column: "ElectionRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenReports_Locations_LocationId",
                table: "CitizenReports",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitizenReports_Locations_LocationId",
                table: "CitizenReports");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_CitizenReports_LocationId",
                table: "CitizenReports");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CitizenReports");
        }
    }
}
