using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUsersSeparateTables2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_CSOs_CSOId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CSOId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CSOId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "CSOAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CSOAdmins");

            migrationBuilder.AddColumn<Guid>(
                name: "CSOId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CSOId",
                table: "Users",
                column: "CSOId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_CSOs_CSOId",
                table: "Users",
                column: "CSOId",
                principalTable: "CSOs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
