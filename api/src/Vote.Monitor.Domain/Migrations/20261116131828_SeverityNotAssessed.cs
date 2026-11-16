using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <summary>
    /// A new lowest severity, <c>NotAssessed</c>, where every record starts: a record without a stored severity is now
    /// NotAssessed instead of None (None becomes a severity someone chose). The report-list functions
    /// (<c>Get*Entries</c>), <c>"GetRecordSeverity"</c> and <c>"GetReviewMetrics"</c> default a missing severity in two
    /// fragments, rewritten here in place in every function that has them (so the functions' other text, which a later
    /// migration may already have changed, stays as it is). Stored rows keep their value.
    /// Migrations that redefine these functions from now on must default to <c>'NotAssessed'</c>.
    /// </summary>
    public partial class SeverityNotAssessed : Migration
    {
        private static string Rewrite(string from, string to) =>
            $$"""
              DO $do$
              DECLARE
                  fn RECORD;
                  definition TEXT;
                  rewritten TEXT;
              BEGIN
                  FOR fn IN
                      SELECT p.oid
                      FROM pg_proc p
                               INNER JOIN pg_namespace n ON n.oid = p.pronamespace
                      WHERE n.nspname = 'public'
                        AND (p.prosrc LIKE '%"Severity", ''{{from}}'')%' OR p.prosrc LIKE '%= recordId), ''{{from}}'')%')
                  LOOP
                      definition := pg_get_functiondef(fn.oid);
                      rewritten := replace(replace(definition,
                                                   '"Severity", ''{{from}}'')', '"Severity", ''{{to}}'')'),
                                           '= recordId), ''{{from}}'')', '= recordId), ''{{to}}'')');
                      IF rewritten <> definition THEN
                          EXECUTE rewritten;
                      END IF;
                  END LOOP;
              END
              $do$;
              """;

        private static string Rank(params string[] levels) =>
            $$"""
              -- Orders a severity by level, lowest first.
              CREATE OR REPLACE FUNCTION "SeverityRank"(
                  severity TEXT
              )
                  RETURNS INTEGER
              AS
              $$
              SELECT array_position(ARRAY[{{string.Join(", ", levels.Select(x => $"'{x}'"))}}]::text[], severity)
              $$ LANGUAGE sql IMMUTABLE;
              """;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(Rewrite("None", "NotAssessed"));
            migrationBuilder.Sql(Rank("NotAssessed", "None", "Low", "Medium", "High", "Urgent"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(Rank("None", "Low", "Medium", "High", "Urgent"));
            migrationBuilder.Sql(Rewrite("NotAssessed", "None"));
            migrationBuilder.Sql("""UPDATE "RecordSeverities" SET "Severity" = 'None' WHERE "Severity" = 'NotAssessed';""");
        }
    }
}
