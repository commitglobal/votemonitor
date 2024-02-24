using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToNgo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringNGOs_CSOs_NgoId",
                table: "MonitoringNGOs");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObservers_CSOs_InviterNgoId",
                table: "MonitoringObservers");

            migrationBuilder.DropTable(
                name: "CSOAdmins");

            migrationBuilder.DropTable(
                name: "CSOs");

            migrationBuilder.CreateTable(
                name: "Ngos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ngos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NgoAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NgoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NgoAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NgoAdmins_Ngos_NgoId",
                        column: x => x.NgoId,
                        principalTable: "Ngos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NgoAdmins_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NgoAdmins_NgoId",
                table: "NgoAdmins",
                column: "NgoId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringNGOs_Ngos_NgoId",
                table: "MonitoringNGOs",
                column: "NgoId",
                principalTable: "Ngos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObservers_Ngos_InviterNgoId",
                table: "MonitoringObservers",
                column: "InviterNgoId",
                principalTable: "Ngos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringNGOs_Ngos_NgoId",
                table: "MonitoringNGOs");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObservers_Ngos_InviterNgoId",
                table: "MonitoringObservers");

            migrationBuilder.DropTable(
                name: "NgoAdmins");

            migrationBuilder.DropTable(
                name: "Ngos");

            migrationBuilder.CreateTable(
                name: "CSOs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSOs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CSOAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CSOId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSOAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CSOAdmins_CSOs_CSOId",
                        column: x => x.CSOId,
                        principalTable: "CSOs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CSOAdmins_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CSOAdmins_CSOId",
                table: "CSOAdmins",
                column: "CSOId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringNGOs_CSOs_NgoId",
                table: "MonitoringNGOs",
                column: "NgoId",
                principalTable: "CSOs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObservers_CSOs_InviterNgoId",
                table: "MonitoringObservers",
                column: "InviterNgoId",
                principalTable: "CSOs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
