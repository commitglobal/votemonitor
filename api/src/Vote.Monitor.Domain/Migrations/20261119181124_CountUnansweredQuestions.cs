using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <summary>
    /// Answered and unanswered questions follow display logic (docs: advanced filters, section 6): a question with
    /// display logic counts only when its parent's answer meets the rule (one level, as the mobile app decides it).
    /// Adds <c>"NumberOfUnansweredQuestions"</c> (the shown questions left unanswered) to form submissions, polling
    /// station information, incident reports and citizen reports, and recounts both counts of every stored record with
    /// the rules of <c>DisplayLogicEvaluator</c> (a temporary SQL twin, dropped afterwards).
    /// The report-list functions (<c>"GetFormSubmissionEntries"</c>, <c>"GetIncidentReportEntries"</c>,
    /// <c>"GetCitizenReportEntries"</c>) return the new count as a last column and derive <c>"QuestionsAnswered"</c>
    /// from it (All = nothing shown left unanswered) instead of comparing with the form's number of questions; they
    /// are rewritten in place (their other text, which a later migration may change, stays as it is).
    /// </summary>
    public partial class CountUnansweredQuestions : Migration
    {
        /// <summary>The counts of DisplayLogicEvaluator.Count over a form's questions and a record's answers.</summary>
        private const string CountFunction =
            """
            CREATE OR REPLACE FUNCTION "__CountShownAnswers"(questions JSONB, answers JSONB)
                RETURNS INTEGER[]
                LANGUAGE plpgsql
                IMMUTABLE
            AS
            $$
            DECLARE
                q          JSONB;
                logic      JSONB;
                parent     JSONB;
                answer     JSONB;
                cond       TEXT;
                val        TEXT;
                option_ids TEXT[];
                picked     TEXT[];
                cmp        INTEGER;
                shown      BOOLEAN;
                empty      BOOLEAN;
                answered   INTEGER := 0;
                unanswered INTEGER := 0;
            BEGIN
                IF questions IS NULL OR jsonb_typeof(questions) <> 'array' THEN
                    RETURN ARRAY [0, 0];
                END IF;
                IF answers IS NULL OR jsonb_typeof(answers) <> 'array' THEN
                    answers := '[]'::jsonb;
                END IF;

                FOR q IN SELECT x FROM jsonb_array_elements(questions) x
                LOOP
                    logic := q -> 'DisplayLogic';
                    shown := TRUE;

                    IF logic IS NOT NULL AND jsonb_typeof(logic) = 'object' THEN
                        shown := FALSE;
                        parent := NULL;
                        SELECT a INTO parent
                        FROM jsonb_array_elements(answers) a
                        WHERE lower(a ->> 'QuestionId') = lower(logic ->> 'ParentQuestionId')
                        LIMIT 1;

                        cond := logic ->> 'Condition';
                        val := NULLIF(logic ->> 'Value', '');
                        option_ids := CASE
                                          WHEN jsonb_typeof(logic -> 'OptionIds') = 'array'
                                              THEN ARRAY(SELECT lower(o) FROM jsonb_array_elements_text(logic -> 'OptionIds') o)
                                          ELSE '{}'::text[] END;

                        IF parent IS NOT NULL AND cond IS NOT NULL AND (val IS NOT NULL OR cardinality(option_ids) > 0) THEN
                            CASE parent ->> '$answerType'
                                WHEN 'singleSelectAnswer', 'multiSelectAnswer' THEN
                                    IF parent ->> '$answerType' = 'singleSelectAnswer' THEN
                                        picked := array_remove(ARRAY [lower(parent -> 'Selection' ->> 'OptionId')], NULL);
                                    ELSIF jsonb_typeof(parent -> 'Selection') = 'array' THEN
                                        picked := array_remove(ARRAY(SELECT lower(o ->> 'OptionId')
                                                                     FROM jsonb_array_elements(parent -> 'Selection') o), NULL);
                                    ELSE
                                        picked := '{}'::text[];
                                    END IF;

                                    IF cond = 'AnyOf' THEN
                                        shown := picked && option_ids;
                                    ELSIF cond = 'All' THEN
                                        shown := cardinality(option_ids) > 0 AND option_ids <@ picked;
                                    ELSE
                                        -- Older rules name a single option in value.
                                        shown := val IS NOT NULL AND lower(btrim(val)) = ANY (picked);
                                    END IF;
                                WHEN 'numberAnswer', 'ratingAnswer' THEN
                                    -- Compared as numbers; a value that isn't a number never meets the rule.
                                    IF val ~ '^\s*[-+]?([0-9]+(\.[0-9]*)?|\.[0-9]+)([eE][-+]?[0-9]+)?\s*$'
                                        AND (parent ->> 'Value') ~ '^\s*[-+]?[0-9]+(\.[0-9]*)?\s*$' THEN
                                        cmp := sign((parent ->> 'Value')::numeric - btrim(val)::numeric)::integer;
                                    ELSE
                                        cmp := NULL;
                                    END IF;
                                WHEN 'textAnswer' THEN
                                    cmp := CASE
                                               WHEN COALESCE(parent ->> 'Text', '') COLLATE "C" < COALESCE(val, '') COLLATE "C" THEN -1
                                               WHEN COALESCE(parent ->> 'Text', '') COLLATE "C" > COALESCE(val, '') COLLATE "C" THEN 1
                                               ELSE 0 END;
                                ELSE
                                    cmp := NULL;
                            END CASE;

                            IF parent ->> '$answerType' IN ('numberAnswer', 'ratingAnswer', 'textAnswer') THEN
                                shown := cmp IS NOT NULL AND CASE cond
                                    WHEN 'Equals' THEN cmp = 0
                                    WHEN 'NotEquals' THEN cmp <> 0
                                    WHEN 'GreaterThan' THEN cmp > 0
                                    WHEN 'GreaterEqual' THEN cmp >= 0
                                    WHEN 'LessThan' THEN cmp < 0
                                    WHEN 'LessEqual' THEN cmp <= 0
                                    ELSE FALSE END;
                            END IF;
                        END IF;
                    END IF;

                    IF shown THEN
                        answer := NULL;
                        SELECT a INTO answer
                        FROM jsonb_array_elements(answers) a
                        WHERE lower(a ->> 'QuestionId') = lower(q ->> 'Id')
                        LIMIT 1;

                        empty := CASE
                                     WHEN answer IS NULL THEN TRUE
                                     ELSE CASE answer ->> '$answerType'
                                              WHEN 'textAnswer' THEN COALESCE(answer ->> 'Text', '') ~ '^\s*$'
                                              WHEN 'numberAnswer' THEN answer ->> 'Value' IS NULL
                                              WHEN 'ratingAnswer' THEN answer ->> 'Value' IS NULL
                                              WHEN 'dateAnswer' THEN NULLIF(answer ->> 'Date', '') IS NULL
                                              WHEN 'timeAnswer' THEN NULLIF(answer ->> 'Time', '') IS NULL
                                              WHEN 'dateTimeAnswer' THEN NULLIF(answer ->> 'DateTime', '') IS NULL
                                              WHEN 'singleSelectAnswer' THEN
                                                  COALESCE(answer -> 'Selection' ->> 'OptionId', '00000000-0000-0000-0000-000000000000')
                                                      = '00000000-0000-0000-0000-000000000000'
                                              WHEN 'multiSelectAnswer' THEN
                                                  jsonb_typeof(answer -> 'Selection') IS DISTINCT FROM 'array'
                                                      OR jsonb_array_length(answer -> 'Selection') = 0
                                              ELSE FALSE END
                            END;

                        IF empty THEN
                            unanswered := unanswered + 1;
                        ELSE
                            answered := answered + 1;
                        END IF;
                    END IF;
                END LOOP;

                RETURN ARRAY [answered, unanswered];
            END;
            $$;
            """;

        private const string Recount =
            """
            UPDATE "FormSubmissions" R
            SET "NumberOfQuestionsAnswered" = C."Counts"[1], "NumberOfUnansweredQuestions" = C."Counts"[2]
            FROM (SELECT X."Id", "__CountShownAnswers"(F."Questions", X."Answers") AS "Counts"
                  FROM "FormSubmissions" X INNER JOIN "Forms" F ON F."Id" = X."FormId") C
            WHERE C."Id" = R."Id";

            UPDATE "IncidentReports" R
            SET "NumberOfQuestionsAnswered" = C."Counts"[1], "NumberOfUnansweredQuestions" = C."Counts"[2]
            FROM (SELECT X."Id", "__CountShownAnswers"(F."Questions", X."Answers") AS "Counts"
                  FROM "IncidentReports" X INNER JOIN "Forms" F ON F."Id" = X."FormId") C
            WHERE C."Id" = R."Id";

            UPDATE "CitizenReports" R
            SET "NumberOfQuestionsAnswered" = C."Counts"[1], "NumberOfUnansweredQuestions" = C."Counts"[2]
            FROM (SELECT X."Id", "__CountShownAnswers"(F."Questions", X."Answers") AS "Counts"
                  FROM "CitizenReports" X INNER JOIN "Forms" F ON F."Id" = X."FormId") C
            WHERE C."Id" = R."Id";

            UPDATE "PollingStationInformation" R
            SET "NumberOfQuestionsAnswered" = C."Counts"[1], "NumberOfUnansweredQuestions" = C."Counts"[2]
            FROM (SELECT X."Id", "__CountShownAnswers"(F."Questions", X."Answers") AS "Counts"
                  FROM "PollingStationInformation" X
                      INNER JOIN "PollingStationInformationForms" F ON F."Id" = X."PollingStationInformationFormId") C
            WHERE C."Id" = R."Id";
            """;

        /// <summary>
        /// Rewrites a function's definition in place, failing when a fragment isn't there; the function is dropped
        /// first, since its result columns may change.
        /// </summary>
        private static string Rewrite(string function, params (string From, string To)[] replacements)
        {
            static string Literal(string value) => "'" + value.Replace("'", "''") + "'";

            var steps = string.Join("\n", replacements.Select(x =>
                $$"""
                      IF position({{Literal(x.From)}} IN rewritten) = 0 THEN
                          RAISE EXCEPTION 'Fragment not found in %: %', {{Literal(function)}}, {{Literal(x.From)}};
                      END IF;
                      rewritten := replace(rewritten, {{Literal(x.From)}}, {{Literal(x.To)}});
                  """));

            return $$"""
                     DO $do$
                     DECLARE
                         fn OID;
                         rewritten TEXT;
                     BEGIN
                         SELECT p.oid INTO fn
                         FROM pg_proc p
                                  INNER JOIN pg_namespace n ON n.oid = p.pronamespace
                         WHERE n.nspname = 'public'
                           AND p.proname = {{Literal(function)}};

                         rewritten := pg_get_functiondef(fn);
                     {{steps}}
                         EXECUTE 'DROP FUNCTION ' || fn::regprocedure::text;
                         EXECUTE rewritten;
                     END
                     $do$;
                     """;
        }

        private static readonly (string From, string To)[] FormSubmissionEntries =
        [
            ("\"SubmissionCount\" integer)", "\"SubmissionCount\" integer, \"NumberOfUnansweredQuestions\" integer)"),
            ("WHEN psi.\"NumberOfQuestionsAnswered\" = psif.\"NumberOfQuestions\" THEN 'All'::text",
                "WHEN psi.\"NumberOfUnansweredQuestions\" = 0 THEN 'All'::text"),
            ("WHEN fs.\"NumberOfQuestionsAnswered\" = f.\"NumberOfQuestions\" THEN 'All'::text",
                "WHEN fs.\"NumberOfUnansweredQuestions\" = 0 THEN 'All'::text"),
            ("1 AS \"SubmissionCount\"", "1 AS \"SubmissionCount\", psi.\"NumberOfUnansweredQuestions\""),
            (")::INTEGER AS \"SubmissionCount\"", ")::INTEGER AS \"SubmissionCount\", fs.\"NumberOfUnansweredQuestions\"")
        ];

        private static readonly (string From, string To)[] IncidentReportEntries =
        [
            ("\"Labels\" text[])", "\"Labels\" text[], \"NumberOfUnansweredQuestions\" integer)"),
            ("WHEN F.\"NumberOfQuestions\" = IR.\"NumberOfQuestionsAnswered\" THEN 'All'::text",
                "WHEN IR.\"NumberOfUnansweredQuestions\" = 0 THEN 'All'::text"),
            ("RLB.\"Value\" AS \"Labels\"", "RLB.\"Value\" AS \"Labels\", IR.\"NumberOfUnansweredQuestions\"")
        ];

        private static readonly (string From, string To)[] CitizenReportEntries =
        [
            ("\"Labels\" text[])", "\"Labels\" text[], \"NumberOfUnansweredQuestions\" integer)"),
            ("WHEN F.\"NumberOfQuestions\" = CR.\"NumberOfQuestionsAnswered\" THEN 'All'::text",
                "WHEN CR.\"NumberOfUnansweredQuestions\" = 0 THEN 'All'::text"),
            ("RLB.\"Value\" AS \"Labels\"", "RLB.\"Value\" AS \"Labels\", CR.\"NumberOfUnansweredQuestions\"")
        ];

        private static (string From, string To)[] Reverse((string From, string To)[] replacements) =>
            replacements.Select(x => (x.To, x.From)).ToArray();

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfUnansweredQuestions",
                table: "PollingStationInformation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfUnansweredQuestions",
                table: "IncidentReports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfUnansweredQuestions",
                table: "FormSubmissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfUnansweredQuestions",
                table: "CitizenReports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(CountFunction);
            migrationBuilder.Sql(Recount);
            migrationBuilder.Sql("""DROP FUNCTION "__CountShownAnswers"(JSONB, JSONB);""");

            migrationBuilder.Sql(Rewrite("GetFormSubmissionEntries", FormSubmissionEntries));
            migrationBuilder.Sql(Rewrite("GetIncidentReportEntries", IncidentReportEntries));
            migrationBuilder.Sql(Rewrite("GetCitizenReportEntries", CitizenReportEntries));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(Rewrite("GetCitizenReportEntries", Reverse(CitizenReportEntries)));
            migrationBuilder.Sql(Rewrite("GetIncidentReportEntries", Reverse(IncidentReportEntries)));
            migrationBuilder.Sql(Rewrite("GetFormSubmissionEntries", Reverse(FormSubmissionEntries)));

            // "NumberOfQuestionsAnswered" keeps its display-logic count: the previous count (every answer) can't be
            // told apart from it once answers change.

            migrationBuilder.DropColumn(
                name: "NumberOfUnansweredQuestions",
                table: "PollingStationInformation");

            migrationBuilder.DropColumn(
                name: "NumberOfUnansweredQuestions",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "NumberOfUnansweredQuestions",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "NumberOfUnansweredQuestions",
                table: "CitizenReports");
        }
    }
}
