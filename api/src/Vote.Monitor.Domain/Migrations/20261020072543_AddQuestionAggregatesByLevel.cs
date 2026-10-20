using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionAggregatesByLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                -- Aggregates the answers to one question, broken down by the locations of one level (1..5): one location per distinct
                -- Level1..<level> path (keyed by the full path because names can repeat under different parents). Submissions without a
                -- value at <level> are left out of the breakdown.
                -- Returns {
                --   "Aggregate": <the question's aggregate over all p_submissions, with the question (see "ReferenceAnswers")>,
                --   "Locations": [{ "Level1".."Level5", "SubmissionCount", "TotalNumberOfQuestionsAnswered", "TotalNumberOfFlaggedAnswers",
                --                   "PollingStationsCount", "LocationsCount", "Aggregate" }],
                --   "Texts", "Notes", "Attachments": numbered ("Id") and referenced by id from the aggregates
                -- }
                -- p_question: the question as stored; p_display_order: its position in the form.
                -- p_submissions: shaped as for "AggregateAnswersByLocation"; only their answers to the question are needed.
                -- p_notes, p_attachments: the notes / attachments of p_submissions for the question.
                CREATE OR REPLACE FUNCTION "AggregateQuestionByLevel"(
                    p_question JSONB,
                    p_display_order INTEGER,
                    p_level INTEGER,
                    p_submissions JSONB,
                    p_notes JSONB,
                    p_attachments JSONB
                )
                    RETURNS JSONB
                AS
                $$
                DECLARE
                    v_question_id TEXT := p_question ->> 'Id';
                    v_questions   JSONB := JSONB_BUILD_ARRAY(p_question);
                    v_aggregates  JSONB;
                    v_texts       JSONB;
                    v_notes       JSONB;
                    v_attachments JSONB;
                    v_locations   JSONB;
                BEGIN
                    v_aggregates := JSONB_SET("AggregateAnswers"(v_questions, p_submissions),
                                              ARRAY [v_question_id, 'DisplayOrder'], TO_JSONB(p_display_order));
                    v_texts := "ExtractTexts"(v_aggregates);
                    v_notes := "NumberItems"(p_notes);
                    v_attachments := "NumberItems"(p_attachments);

                    WITH
                        S AS (
                            SELECT SUB                                                          AS submission,
                                   NULLIF(SUB ->> 'Level1', '')                                 AS l1,
                                   CASE WHEN p_level >= 2 THEN NULLIF(SUB ->> 'Level2', '') END AS l2,
                                   CASE WHEN p_level >= 3 THEN NULLIF(SUB ->> 'Level3', '') END AS l3,
                                   CASE WHEN p_level >= 4 THEN NULLIF(SUB ->> 'Level4', '') END AS l4,
                                   CASE WHEN p_level >= 5 THEN NULLIF(SUB ->> 'Level5', '') END AS l5
                            FROM JSONB_ARRAY_ELEMENTS(p_submissions) SUB
                        ),
                        GROUPS AS (
                            SELECT l1, l2, l3, l4, l5, JSONB_AGG(submission) AS submissions
                            FROM S
                            WHERE (ARRAY [l1, l2, l3, l4, l5])[p_level] IS NOT NULL
                            GROUP BY l1, l2, l3, l4, l5
                        ),
                        LOCATIONS AS (
                            SELECT G.l1, G.l2, G.l3, G.l4, G.l5,
                                   "AggregateLocationGroup"(v_questions, G.submissions, v_texts -> 'Ids', v_notes, v_attachments) AS stats
                            FROM GROUPS G
                        )
                    SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                                      'Level1', l1, 'Level2', l2, 'Level3', l3, 'Level4', l4, 'Level5', l5,
                                                      'Aggregate', JSONB_SET(stats -> 'Aggregates' -> v_question_id, '{DisplayOrder}', TO_JSONB(p_display_order)))
                                                  || (stats - 'Aggregates')
                                              ORDER BY l1, l2, l3, l4, l5), '[]'::JSONB)
                    INTO v_locations
                    FROM LOCATIONS;

                    RETURN JSONB_BUILD_OBJECT(
                            'Aggregate', "ReferenceAnswers"(v_aggregates, v_texts -> 'Ids', v_notes, v_attachments, TRUE) -> v_question_id,
                            'Locations', v_locations,
                            'Texts', v_texts -> 'Texts',
                            'Notes', v_notes,
                            'Attachments', v_attachments);
                END;
                $$ LANGUAGE plpgsql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Aggregates the answers to one question of a form's submissions, broken down by the locations of one level (1..5)
                -- (see "AggregateQuestionByLevel"). Returns one row shaped like QuestionAggregateByLevel, or no rows when the form is not
                -- available to the ngo or does not contain the question.
                --
                -- filter is a SQL predicate over "GetFormSubmissionEntries" aliased as s, built server-side by FilterSqlCompiler.
                -- Its values are read from filterParameters as ($1 ->> 'name'), so they are never part of the SQL text.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionsQuestionAggregateByLevel"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    formId UUID,
                    questionId UUID,
                    level INTEGER,
                    filter TEXT DEFAULT 'TRUE',
                    filterParameters JSONB DEFAULT '{}'::JSONB
                )
                    RETURNS TABLE
                            (
                                "FormId"               UUID,
                                "FormCode"             TEXT,
                                "FormType"             TEXT,
                                "DefaultLanguage"      TEXT,
                                "Languages"            TEXT[],
                                "Level"                TEXT,
                                "SubmissionCount"      INTEGER,
                                "PollingStationsCount" INTEGER,
                                "LocationsCount"       INTEGER,
                                "Aggregate"            JSONB,
                                "Locations"            JSONB,
                                "Notes"                JSONB,
                                "Attachments"          JSONB,
                                "Texts"                JSONB
                            )
                AS
                $$
                #variable_conflict use_column
                DECLARE
                    v_form          RECORD;
                    v_question      JSONB;
                    v_display_order INTEGER;
                    v_collected     RECORD;
                    v_result        JSONB;
                BEGIN
                    -- load the form: a published form available to the ngo, or the election round's PSI form
                    SELECT F."Id"                    AS id,
                           F."Code"::TEXT            AS code,
                           F."FormType"              AS form_type,
                           F."DefaultLanguage"::TEXT AS default_language,
                           F."Languages"             AS languages,
                           F."Questions"             AS questions
                    INTO v_form
                    FROM "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF
                             INNER JOIN "Forms" F ON F."Id" = AF."FormId"
                    WHERE F."Id" = formId
                      AND F."Status" = 'Published'
                      AND F."FormType" <> 'CitizenReporting';

                    IF NOT FOUND THEN
                        SELECT PSIF."Id"                    AS id,
                               'PSI'                        AS code,
                               'PSI'                        AS form_type,
                               PSIF."DefaultLanguage"::TEXT AS default_language,
                               PSIF."Languages"             AS languages,
                               PSIF."Questions"             AS questions
                        INTO v_form
                        FROM "PollingStationInformationForms" PSIF
                        WHERE PSIF."ElectionRoundId" = electionRoundId
                          AND PSIF."Id" = formId;

                        IF NOT FOUND THEN
                            RETURN;
                        END IF;
                    END IF;

                    SELECT Q.question, Q.idx - 1
                    INTO v_question, v_display_order
                    FROM JSONB_ARRAY_ELEMENTS(v_form.questions) WITH ORDINALITY Q(question, idx)
                    WHERE Q.question ->> 'Id' = questionId::TEXT;

                    IF v_question IS NULL THEN
                        RETURN;
                    END IF;

                    -- load the submissions matching the filter, with their answers, notes and attachments for the question
                    EXECUTE FORMAT(
                            $sql$
                            WITH
                                SUBMISSIONS AS MATERIALIZED (
                                    SELECT S."SubmissionId",
                                           S."TimeSubmitted",
                                           S."MonitoringObserverId",
                                           S."PollingStationId",
                                           S."Level1",
                                           S."Level2",
                                           S."Level3",
                                           S."Level4",
                                           S."Level5",
                                           S."NumberOfQuestionsAnswered",
                                           S."NumberOfFlaggedAnswers",
                                           S."Answers",
                                           S."Notes",
                                           S."Attachments"
                                    FROM "GetFormSubmissionEntries"($2, $3, $4) S
                                    WHERE S."FormId" = $5
                                      AND (%s)
                                )
                            SELECT
                                (SELECT COUNT(*) FROM SUBMISSIONS)::INT AS submission_count,

                                (SELECT COUNT(DISTINCT "PollingStationId") FROM SUBMISSIONS)::INT AS polling_stations,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', "SubmissionId",
                                        'ResponderId', "MonitoringObserverId",
                                        'TimeSubmitted', "TimeSubmitted",
                                        'Answers', (SELECT COALESCE(JSONB_AGG(A), '[]'::JSONB)
                                                    FROM JSONB_ARRAY_ELEMENTS(COALESCE("Answers", '[]'::JSONB)) A
                                                    WHERE A ->> 'QuestionId' = $6::TEXT),
                                        'NumberOfQuestionsAnswered', "NumberOfQuestionsAnswered",
                                        'NumberOfFlaggedAnswers', "NumberOfFlaggedAnswers",
                                        'Level1', "Level1",
                                        'Level2', "Level2",
                                        'Level3', "Level3",
                                        'Level4', "Level4",
                                        'Level5', "Level5",
                                        'PollingStationId', "PollingStationId")), '[]'::JSONB)
                                 FROM SUBMISSIONS) AS submissions,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'QuestionId', N -> 'QuestionId',
                                        'SubmissionId', S."SubmissionId",
                                        'Text', N -> 'Text',
                                        'TimeSubmitted', N -> 'TimeSubmitted',
                                        'MonitoringObserverId', S."MonitoringObserverId")), '[]'::JSONB)
                                 FROM SUBMISSIONS S
                                          CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Notes") N
                                 WHERE N ->> 'QuestionId' = $6::TEXT) AS notes,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', S."SubmissionId",
                                        'QuestionId', F -> 'QuestionId',
                                        'MonitoringObserverId', S."MonitoringObserverId",
                                        'FilePath', F -> 'FilePath',
                                        'UploadedFileName', F -> 'UploadedFileName',
                                        'FileName', F -> 'FileName',
                                        'MimeType', F -> 'MimeType',
                                        'TimeSubmitted', F -> 'TimeSubmitted')), '[]'::JSONB)
                                 FROM SUBMISSIONS S
                                          CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Attachments") F
                                 WHERE F ->> 'QuestionId' = $6::TEXT) AS attachments
                            $sql$, filter)
                    INTO v_collected
                    USING filterParameters, electionRoundId, ngoId, dataSource, formId, questionId;

                    v_result := "AggregateQuestionByLevel"(v_question, v_display_order, level, v_collected.submissions,
                                                           v_collected.notes, v_collected.attachments);

                    RETURN QUERY
                        SELECT v_form.id,
                               v_form.code,
                               v_form.form_type,
                               v_form.default_language,
                               v_form.languages,
                               'Level' || level,
                               v_collected.submission_count,
                               v_collected.polling_stations,
                               NULL::INTEGER,
                               v_result -> 'Aggregate',
                               v_result -> 'Locations',
                               v_result -> 'Notes',
                               v_result -> 'Attachments',
                               v_result -> 'Texts';
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                -- Aggregates the answers to one question of an incident reporting form, broken down by the locations of one level (1..5)
                -- (see "AggregateQuestionByLevel"). Returns one row shaped like QuestionAggregateByLevel, or no rows when the form does not
                -- belong to the ngo or does not contain the question.
                --
                -- filter is a SQL predicate over "GetIncidentReportEntries" aliased as s, built server-side by FilterSqlCompiler.
                -- Its values are read from filterParameters as ($1 ->> 'name'), so they are never part of the SQL text.
                CREATE OR REPLACE FUNCTION "GetIncidentReportsQuestionAggregateByLevel"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    formId UUID,
                    questionId UUID,
                    level INTEGER,
                    filter TEXT DEFAULT 'TRUE',
                    filterParameters JSONB DEFAULT '{}'::JSONB
                )
                    RETURNS TABLE
                            (
                                "FormId"               UUID,
                                "FormCode"             TEXT,
                                "FormType"             TEXT,
                                "DefaultLanguage"      TEXT,
                                "Languages"            TEXT[],
                                "Level"                TEXT,
                                "SubmissionCount"      INTEGER,
                                "PollingStationsCount" INTEGER,
                                "LocationsCount"       INTEGER,
                                "Aggregate"            JSONB,
                                "Locations"            JSONB,
                                "Notes"                JSONB,
                                "Attachments"          JSONB,
                                "Texts"                JSONB
                            )
                AS
                $$
                #variable_conflict use_column
                DECLARE
                    v_form          RECORD;
                    v_question      JSONB;
                    v_display_order INTEGER;
                    v_collected     RECORD;
                    v_result        JSONB;
                BEGIN
                    -- load the form: a form of the ngo in this election round
                    SELECT F."Id"                    AS id,
                           F."Code"::TEXT            AS code,
                           F."FormType"              AS form_type,
                           F."DefaultLanguage"::TEXT AS default_language,
                           F."Languages"             AS languages,
                           F."Questions"             AS questions
                    INTO v_form
                    FROM "Forms" F
                             INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
                    WHERE F."Id" = formId
                      AND F."ElectionRoundId" = electionRoundId
                      AND MN."ElectionRoundId" = electionRoundId
                      AND MN."NgoId" = ngoId;

                    IF NOT FOUND THEN
                        RETURN;
                    END IF;

                    SELECT Q.question, Q.idx - 1
                    INTO v_question, v_display_order
                    FROM JSONB_ARRAY_ELEMENTS(v_form.questions) WITH ORDINALITY Q(question, idx)
                    WHERE Q.question ->> 'Id' = questionId::TEXT;

                    IF v_question IS NULL THEN
                        RETURN;
                    END IF;

                    -- load the incident reports matching the filter, with their answers, notes and attachments for the question
                    EXECUTE FORMAT(
                            $sql$
                            WITH
                                SUBMISSIONS AS MATERIALIZED (
                                    SELECT S."Id" AS "SubmissionId",
                                           S."TimeSubmitted",
                                           S."MonitoringObserverId",
                                           S."PollingStationId",
                                           S."Level1",
                                           S."Level2",
                                           S."Level3",
                                           S."Level4",
                                           S."Level5",
                                           S."NumberOfQuestionsAnswered",
                                           S."NumberOfFlaggedAnswers",
                                           S."Answers"
                                    FROM "GetIncidentReportEntries"($2, $3, $4) S
                                    WHERE S."FormId" = $5
                                      AND (%s)
                                )
                            SELECT
                                (SELECT COUNT(*) FROM SUBMISSIONS)::INT AS submission_count,

                                (SELECT COUNT(DISTINCT "PollingStationId") FROM SUBMISSIONS)::INT AS polling_stations,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', "SubmissionId",
                                        'ResponderId', "MonitoringObserverId",
                                        'TimeSubmitted', "TimeSubmitted",
                                        'Answers', (SELECT COALESCE(JSONB_AGG(A), '[]'::JSONB)
                                                    FROM JSONB_ARRAY_ELEMENTS(COALESCE("Answers", '[]'::JSONB)) A
                                                    WHERE A ->> 'QuestionId' = $6::TEXT),
                                        'NumberOfQuestionsAnswered', "NumberOfQuestionsAnswered",
                                        'NumberOfFlaggedAnswers', "NumberOfFlaggedAnswers",
                                        'Level1', "Level1",
                                        'Level2', "Level2",
                                        'Level3', "Level3",
                                        'Level4', "Level4",
                                        'Level5', "Level5",
                                        'PollingStationId', "PollingStationId")), '[]'::JSONB)
                                 FROM SUBMISSIONS) AS submissions,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'QuestionId', N."QuestionId",
                                        'SubmissionId', S."SubmissionId",
                                        'Text', N."Text",
                                        'TimeSubmitted', COALESCE(N."LastModifiedOn", N."CreatedOn"),
                                        'MonitoringObserverId', S."MonitoringObserverId")), '[]'::JSONB)
                                 FROM SUBMISSIONS S
                                          INNER JOIN "IncidentReportNotes" N ON N."IncidentReportId" = S."SubmissionId"
                                 WHERE N."QuestionId" = $6) AS notes,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', S."SubmissionId",
                                        'QuestionId', A."QuestionId",
                                        'MonitoringObserverId', S."MonitoringObserverId",
                                        'FilePath', A."FilePath",
                                        'UploadedFileName', A."UploadedFileName",
                                        'FileName', A."FileName",
                                        'MimeType', A."MimeType",
                                        'TimeSubmitted', A."CreatedOn")), '[]'::JSONB)
                                 FROM SUBMISSIONS S
                                          INNER JOIN "IncidentReportAttachments" A ON A."IncidentReportId" = S."SubmissionId"
                                 WHERE A."QuestionId" = $6
                                   AND A."IsDeleted" = FALSE
                                   AND A."IsCompleted" = TRUE) AS attachments
                            $sql$, filter)
                    INTO v_collected
                    USING filterParameters, electionRoundId, ngoId, dataSource, formId, questionId;

                    v_result := "AggregateQuestionByLevel"(v_question, v_display_order, level, v_collected.submissions,
                                                           v_collected.notes, v_collected.attachments);

                    RETURN QUERY
                        SELECT v_form.id,
                               v_form.code,
                               v_form.form_type,
                               v_form.default_language,
                               v_form.languages,
                               'Level' || level,
                               v_collected.submission_count,
                               v_collected.polling_stations,
                               NULL::INTEGER,
                               v_result -> 'Aggregate',
                               v_result -> 'Locations',
                               v_result -> 'Notes',
                               v_result -> 'Attachments',
                               v_result -> 'Texts';
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                -- Aggregates the answers to one question of a citizen reporting form, broken down by the locations of one level (1..5)
                -- (see "AggregateQuestionByLevel"). Returns one row shaped like QuestionAggregateByLevel, or no rows when the form does not
                -- belong to the ngo or does not contain the question. Citizen reports have no observer, so responder ids are empty.
                --
                -- filter is a SQL predicate over "GetCitizenReportEntries" aliased as s, built server-side by FilterSqlCompiler.
                -- Its values are read from filterParameters as ($1 ->> 'name'), so they are never part of the SQL text.
                CREATE OR REPLACE FUNCTION "GetCitizenReportsQuestionAggregateByLevel"(
                    electionRoundId UUID,
                    ngoId UUID,
                    formId UUID,
                    questionId UUID,
                    level INTEGER,
                    filter TEXT DEFAULT 'TRUE',
                    filterParameters JSONB DEFAULT '{}'::JSONB
                )
                    RETURNS TABLE
                            (
                                "FormId"               UUID,
                                "FormCode"             TEXT,
                                "FormType"             TEXT,
                                "DefaultLanguage"      TEXT,
                                "Languages"            TEXT[],
                                "Level"                TEXT,
                                "SubmissionCount"      INTEGER,
                                "PollingStationsCount" INTEGER,
                                "LocationsCount"       INTEGER,
                                "Aggregate"            JSONB,
                                "Locations"            JSONB,
                                "Notes"                JSONB,
                                "Attachments"          JSONB,
                                "Texts"                JSONB
                            )
                AS
                $$
                #variable_conflict use_column
                DECLARE
                    v_form          RECORD;
                    v_question      JSONB;
                    v_display_order INTEGER;
                    v_collected     RECORD;
                    v_result        JSONB;
                BEGIN
                    -- load the form: a form of the ngo in this election round
                    SELECT F."Id"                    AS id,
                           F."Code"::TEXT            AS code,
                           F."FormType"              AS form_type,
                           F."DefaultLanguage"::TEXT AS default_language,
                           F."Languages"             AS languages,
                           F."Questions"             AS questions
                    INTO v_form
                    FROM "Forms" F
                             INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
                    WHERE F."Id" = formId
                      AND F."ElectionRoundId" = electionRoundId
                      AND MN."ElectionRoundId" = electionRoundId
                      AND MN."NgoId" = ngoId;

                    IF NOT FOUND THEN
                        RETURN;
                    END IF;

                    SELECT Q.question, Q.idx - 1
                    INTO v_question, v_display_order
                    FROM JSONB_ARRAY_ELEMENTS(v_form.questions) WITH ORDINALITY Q(question, idx)
                    WHERE Q.question ->> 'Id' = questionId::TEXT;

                    IF v_question IS NULL THEN
                        RETURN;
                    END IF;

                    -- load the citizen reports matching the filter, with their answers, notes and attachments for the question
                    EXECUTE FORMAT(
                            $sql$
                            WITH
                                SUBMISSIONS AS MATERIALIZED (
                                    SELECT S."Id" AS "SubmissionId",
                                           S."TimeSubmitted",
                                           S."LocationId",
                                           S."Level1",
                                           S."Level2",
                                           S."Level3",
                                           S."Level4",
                                           S."Level5",
                                           S."NumberOfQuestionsAnswered",
                                           S."NumberOfFlaggedAnswers",
                                           S."Answers"
                                    FROM "GetCitizenReportEntries"($2, $3) S
                                    WHERE S."FormId" = $4
                                      AND (%s)
                                )
                            SELECT
                                (SELECT COUNT(*) FROM SUBMISSIONS)::INT AS submission_count,

                                (SELECT COUNT(DISTINCT "LocationId") FROM SUBMISSIONS)::INT AS locations,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', "SubmissionId",
                                        'ResponderId', '00000000-0000-0000-0000-000000000000'::UUID,
                                        'TimeSubmitted', "TimeSubmitted",
                                        'Answers', (SELECT COALESCE(JSONB_AGG(A), '[]'::JSONB)
                                                    FROM JSONB_ARRAY_ELEMENTS(COALESCE("Answers", '[]'::JSONB)) A
                                                    WHERE A ->> 'QuestionId' = $5::TEXT),
                                        'NumberOfQuestionsAnswered', "NumberOfQuestionsAnswered",
                                        'NumberOfFlaggedAnswers', "NumberOfFlaggedAnswers",
                                        'Level1', "Level1",
                                        'Level2', "Level2",
                                        'Level3', "Level3",
                                        'Level4', "Level4",
                                        'Level5', "Level5",
                                        'LocationId', "LocationId")), '[]'::JSONB)
                                 FROM SUBMISSIONS) AS submissions,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'QuestionId', N."QuestionId",
                                        'SubmissionId', S."SubmissionId",
                                        'Text', N."Text",
                                        'TimeSubmitted', COALESCE(N."LastModifiedOn", N."CreatedOn"))), '[]'::JSONB)
                                 FROM SUBMISSIONS S
                                          INNER JOIN "CitizenReportNotes" N ON N."CitizenReportId" = S."SubmissionId"
                                 WHERE N."QuestionId" = $5) AS notes,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', S."SubmissionId",
                                        'QuestionId', A."QuestionId",
                                        'FilePath', A."FilePath",
                                        'UploadedFileName', A."UploadedFileName",
                                        'FileName', A."FileName",
                                        'MimeType', A."MimeType",
                                        'TimeSubmitted', A."CreatedOn")), '[]'::JSONB)
                                 FROM SUBMISSIONS S
                                          INNER JOIN "CitizenReportAttachments" A ON A."CitizenReportId" = S."SubmissionId"
                                 WHERE A."QuestionId" = $5
                                   AND A."IsDeleted" = FALSE
                                   AND A."IsCompleted" = TRUE) AS attachments
                            $sql$, filter)
                    INTO v_collected
                    USING filterParameters, electionRoundId, ngoId, formId, questionId;

                    v_result := "AggregateQuestionByLevel"(v_question, v_display_order, level, v_collected.submissions,
                                                           v_collected.notes, v_collected.attachments);

                    RETURN QUERY
                        SELECT v_form.id,
                               v_form.code,
                               v_form.form_type,
                               v_form.default_language,
                               v_form.languages,
                               'Level' || level,
                               v_collected.submission_count,
                               NULL::INTEGER,
                               v_collected.locations,
                               v_result -> 'Aggregate',
                               v_result -> 'Locations',
                               v_result -> 'Notes',
                               v_result -> 'Attachments',
                               v_result -> 'Texts';
                END;
                $$ LANGUAGE plpgsql;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportsQuestionAggregateByLevel"(uuid, uuid, uuid, uuid, integer, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportsQuestionAggregateByLevel"(uuid, uuid, text, uuid, uuid, integer, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionsQuestionAggregateByLevel"(uuid, uuid, text, uuid, uuid, integer, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "AggregateQuestionByLevel"(jsonb, integer, integer, jsonb, jsonb, jsonb);""");
        }
    }
}
