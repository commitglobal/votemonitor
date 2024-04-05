using Microsoft.EntityFrameworkCore.Migrations;
using Vote.Monitor.Domain.Constants;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddArrayUniqueFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
            CREATE OR REPLACE FUNCTION ""array_unique"" (a text[]) RETURNS text[] AS $$
              SELECT ARRAY (
                SELECT DISTINCT v FROM unnest(a) AS b(v)
              )
            $$ LANGUAGE SQL;
              ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"DROP FUNCTION IF EXISTS ""array_unique"";");
        }
    }
}
