using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddSubStagesAndAnswerHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChangedAfterReviewAt",
                table: "ReviewWorkflows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ReviewStages",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "ReviewStages",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentStageId",
                table: "ReviewStages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnswerChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousAnswer = table.Column<string>(type: "jsonb", nullable: true),
                    CurrentAnswer = table.Column<string>(type: "jsonb", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerChanges_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStages_ParentStageId",
                table: "ReviewStages",
                column: "ParentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerChanges_ElectionRoundId",
                table: "AnswerChanges",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerChanges_RecordType_RecordId_ChangedAt",
                table: "AnswerChanges",
                columns: new[] { "RecordType", "RecordId", "ChangedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewStages_ReviewStages_ParentStageId",
                table: "ReviewStages",
                column: "ParentStageId",
                principalTable: "ReviewStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewStages_ReviewStages_ParentStageId",
                table: "ReviewStages");

            migrationBuilder.DropTable(
                name: "AnswerChanges");

            migrationBuilder.DropIndex(
                name: "IX_ReviewStages_ParentStageId",
                table: "ReviewStages");

            migrationBuilder.DropColumn(
                name: "ChangedAfterReviewAt",
                table: "ReviewWorkflows");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "ReviewStages");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "ReviewStages");

            migrationBuilder.DropColumn(
                name: "ParentStageId",
                table: "ReviewStages");
        }
    }
}
