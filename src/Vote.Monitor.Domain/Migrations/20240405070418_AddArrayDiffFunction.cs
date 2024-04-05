using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddArrayDiffFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
            CREATE OR REPLACE FUNCTION ""array_diff""(minuend anyarray, subtrahend anyarray, out difference anyarray)
            RETURNS anyarray AS
            $$
            BEGIN
                EXECUTE 'SELECT array(select unnest($1) EXCEPT SELECT unnest($2))'
                USING minuend, subtrahend
                INTO difference;
            END;
            $$ LANGUAGE PLPGSQL RETURNS NULL ON NULL INPUT;
              ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"DROP FUNCTION IF EXISTS ""array_diff"";");
        }
    }
}
