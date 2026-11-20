using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <summary>
    /// Forms, form templates, polling station information forms and dashboards use Postgres's <c>xmin</c> system
    /// column as their concurrency token (concurrent edits contract). <c>xmin</c> exists on every row already, so the
    /// database doesn't change; this migration only records the mapping in the model.
    /// </summary>
    public partial class XminConcurrencyTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
