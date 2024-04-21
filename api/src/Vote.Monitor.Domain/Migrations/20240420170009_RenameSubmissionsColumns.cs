using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameSubmissionsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfAnswers",
                table: "PollingStationInformation",
                newName: "NumberOfQuestionsAnswered");

            migrationBuilder.RenameColumn(
                name: "NumberOfQuestionAnswered",
                table: "FormSubmissions",
                newName: "NumberOfQuestionsAnswered");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfQuestionsAnswered",
                table: "PollingStationInformation",
                newName: "NumberOfAnswers");

            migrationBuilder.RenameColumn(
                name: "NumberOfQuestionsAnswered",
                table: "FormSubmissions",
                newName: "NumberOfQuestionAnswered");
        }
    }
}
