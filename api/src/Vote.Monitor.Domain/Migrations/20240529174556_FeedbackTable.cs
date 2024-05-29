using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "QuickReportAttachments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserFeedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserFeedback = table.Column<string>(type: "text", nullable: false),
                    TimeSubmitted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFeedback_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFeedback_Observers_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFeedback_ElectionRoundId",
                table: "UserFeedback",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFeedback_ObserverId",
                table: "UserFeedback",
                column: "ObserverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFeedback");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "QuickReportAttachments");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
