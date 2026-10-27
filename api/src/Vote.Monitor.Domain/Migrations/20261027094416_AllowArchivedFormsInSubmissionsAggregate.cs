using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AllowArchivedFormsInSubmissionsAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ngo admins keep seeing the aggregated submissions of a form after it is archived (Obsolete)
            migrationBuilder.Sql(
                """
                -- Aggregates the answers of a form's submissions. Returns one row shaped like FormSubmissionsAggregate,
                -- or no rows when the form is not available to the ngo.
                -- Texts, Notes and Attachments are numbered ("Id") and the question aggregates reference them by id
                -- (see "ReferenceAnswers"). Locations lists the Level1..Level5 paths of the submissions and every question
                -- aggregate holds its breakdown per location id in "LocationAggregates" (see "AggregateAnswersByLocation").
                --
                -- filter is a SQL predicate over "GetFormSubmissionEntries" aliased as s, built server-side by FilterSqlCompiler.
                -- Its values are read from filterParameters as ($1 ->> 'name'), so they are never part of the SQL text.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionsAggregate"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    formId UUID,
                    filter TEXT DEFAULT 'TRUE',
                    filterParameters JSONB DEFAULT '{}'::JSONB
                )
                    RETURNS TABLE
                            (
                                "FormId"                         UUID,
                                "FormCode"                       TEXT,
                                "FormType"                       TEXT,
                                "Name"                           JSONB,
                                "Description"                    JSONB,
                                "DefaultLanguage"                TEXT,
                                "Languages"                      TEXT[],
                                "Responders"                     JSONB,
                                "SubmissionCount"                INTEGER,
                                "TotalNumberOfQuestionsAnswered" INTEGER,
                                "TotalNumberOfFlaggedAnswers"    INTEGER,
                                "PollingStationsCount"           INTEGER,
                                "Aggregates"                     JSONB,
                                "Notes"                          JSONB,
                                "Attachments"                    JSONB,
                                "Texts"                          JSONB,
                                "Locations"                      JSONB
                            )
                AS
                $$
                #variable_conflict use_column
                DECLARE
                    v_form        RECORD;
                    v_collected   RECORD;
                    v_aggregates  JSONB;
                    v_texts       JSONB;
                    v_notes       JSONB;
                    v_attachments JSONB;
                    v_locations   JSONB;
                BEGIN
                    -- load the form: a published or archived form available to the ngo, or the election round's PSI form
                    SELECT F."Id"                    AS id,
                           F."Code"::TEXT            AS code,
                           F."FormType"              AS form_type,
                           F."Name"                  AS name,
                           F."Description"           AS description,
                           F."DefaultLanguage"::TEXT AS default_language,
                           F."Languages"             AS languages,
                           F."Questions"             AS questions
                    INTO v_form
                    FROM "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF
                             INNER JOIN "Forms" F ON F."Id" = AF."FormId"
                    WHERE F."Id" = formId
                      AND F."Status" IN ('Published', 'Obsolete')
                      AND F."FormType" <> 'CitizenReporting';

                    IF NOT FOUND THEN
                        SELECT PSIF."Id"                    AS id,
                               'PSI'                        AS code,
                               'PSI'                        AS form_type,
                               L.psi_text                   AS name,
                               L.psi_text                   AS description,
                               PSIF."DefaultLanguage"::TEXT AS default_language,
                               PSIF."Languages"             AS languages,
                               PSIF."Questions"             AS questions
                        INTO v_form
                        FROM "PollingStationInformationForms" PSIF
                                 CROSS JOIN LATERAL (
                            SELECT COALESCE(JSONB_OBJECT_AGG(LANGUAGE, 'PSI'), '{}'::JSONB) AS psi_text
                            FROM UNNEST(PSIF."Languages") LANGUAGE
                            ) L
                        WHERE PSIF."ElectionRoundId" = electionRoundId
                          AND PSIF."Id" = formId;

                        IF NOT FOUND THEN
                            RETURN;
                        END IF;
                    END IF;

                    -- load the submissions matching the filter, with their notes and attachments
                    EXECUTE FORMAT(
                            $sql$
                            WITH
                                SUBMISSIONS AS MATERIALIZED (
                                    SELECT S."SubmissionId",
                                           S."TimeSubmitted",
                                           S."MonitoringObserverId",
                                           S."DisplayName",
                                           S."Email",
                                           S."PhoneNumber",
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

                                (SELECT COALESCE(SUM("NumberOfQuestionsAnswered"), 0) FROM SUBMISSIONS)::INT AS questions_answered,

                                (SELECT COALESCE(SUM("NumberOfFlaggedAnswers"), 0) FROM SUBMISSIONS)::INT AS flagged_answers,

                                (SELECT COUNT(DISTINCT "PollingStationId") FROM SUBMISSIONS)::INT AS polling_stations,

                                (SELECT COALESCE(JSONB_AGG(DISTINCT JSONB_BUILD_OBJECT(
                                        'ResponderId', "MonitoringObserverId",
                                        'DisplayName', "DisplayName",
                                        'Email', "Email",
                                        'PhoneNumber', "PhoneNumber")), '[]'::JSONB)
                                 FROM SUBMISSIONS) AS responders,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', "SubmissionId",
                                        'ResponderId', "MonitoringObserverId",
                                        'TimeSubmitted', "TimeSubmitted",
                                        'Answers', "Answers",
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
                                          CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Notes") N) AS notes,

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
                                          CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Attachments") F) AS attachments
                            $sql$, filter)
                    INTO v_collected
                    USING filterParameters, electionRoundId, ngoId, dataSource, formId;

                    v_aggregates := "AggregateAnswers"(v_form.questions, v_collected.submissions);
                    v_texts := "ExtractTexts"(v_aggregates);
                    v_notes := "NumberItems"(v_collected.notes);
                    v_attachments := "NumberItems"(v_collected.attachments);
                    v_locations := "AggregateAnswersByLocation"(v_form.questions, v_collected.submissions, v_texts -> 'Ids', v_notes, v_attachments);

                    RETURN QUERY
                        SELECT v_form.id,
                               v_form.code,
                               v_form.form_type,
                               v_form.name,
                               v_form.description,
                               v_form.default_language,
                               v_form.languages,
                               v_collected.responders,
                               v_collected.submission_count,
                               v_collected.questions_answered,
                               v_collected.flagged_answers,
                               v_collected.polling_stations,
                               "AttachLocationAggregates"("ReferenceAnswers"(v_aggregates, v_texts -> 'Ids', v_notes, v_attachments, TRUE), v_locations -> 'ByQuestion'),
                               v_notes,
                               v_attachments,
                               v_texts -> 'Texts',
                               v_locations -> 'Locations';
                END;
                $$ LANGUAGE plpgsql;
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
                      AND F."Status" IN ('Published', 'Obsolete')
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                -- Aggregates the answers of a form's submissions. Returns one row shaped like FormSubmissionsAggregate,
                -- or no rows when the form is not available to the ngo.
                -- Texts, Notes and Attachments are numbered ("Id") and the question aggregates reference them by id
                -- (see "ReferenceAnswers"). Locations lists the Level1..Level5 paths of the submissions and every question
                -- aggregate holds its breakdown per location id in "LocationAggregates" (see "AggregateAnswersByLocation").
                --
                -- filter is a SQL predicate over "GetFormSubmissionEntries" aliased as s, built server-side by FilterSqlCompiler.
                -- Its values are read from filterParameters as ($1 ->> 'name'), so they are never part of the SQL text.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionsAggregate"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    formId UUID,
                    filter TEXT DEFAULT 'TRUE',
                    filterParameters JSONB DEFAULT '{}'::JSONB
                )
                    RETURNS TABLE
                            (
                                "FormId"                         UUID,
                                "FormCode"                       TEXT,
                                "FormType"                       TEXT,
                                "Name"                           JSONB,
                                "Description"                    JSONB,
                                "DefaultLanguage"                TEXT,
                                "Languages"                      TEXT[],
                                "Responders"                     JSONB,
                                "SubmissionCount"                INTEGER,
                                "TotalNumberOfQuestionsAnswered" INTEGER,
                                "TotalNumberOfFlaggedAnswers"    INTEGER,
                                "PollingStationsCount"           INTEGER,
                                "Aggregates"                     JSONB,
                                "Notes"                          JSONB,
                                "Attachments"                    JSONB,
                                "Texts"                          JSONB,
                                "Locations"                      JSONB
                            )
                AS
                $$
                #variable_conflict use_column
                DECLARE
                    v_form        RECORD;
                    v_collected   RECORD;
                    v_aggregates  JSONB;
                    v_texts       JSONB;
                    v_notes       JSONB;
                    v_attachments JSONB;
                    v_locations   JSONB;
                BEGIN
                    -- load the form: a published form available to the ngo, or the election round's PSI form
                    SELECT F."Id"                    AS id,
                           F."Code"::TEXT            AS code,
                           F."FormType"              AS form_type,
                           F."Name"                  AS name,
                           F."Description"           AS description,
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
                               L.psi_text                   AS name,
                               L.psi_text                   AS description,
                               PSIF."DefaultLanguage"::TEXT AS default_language,
                               PSIF."Languages"             AS languages,
                               PSIF."Questions"             AS questions
                        INTO v_form
                        FROM "PollingStationInformationForms" PSIF
                                 CROSS JOIN LATERAL (
                            SELECT COALESCE(JSONB_OBJECT_AGG(LANGUAGE, 'PSI'), '{}'::JSONB) AS psi_text
                            FROM UNNEST(PSIF."Languages") LANGUAGE
                            ) L
                        WHERE PSIF."ElectionRoundId" = electionRoundId
                          AND PSIF."Id" = formId;

                        IF NOT FOUND THEN
                            RETURN;
                        END IF;
                    END IF;

                    -- load the submissions matching the filter, with their notes and attachments
                    EXECUTE FORMAT(
                            $sql$
                            WITH
                                SUBMISSIONS AS MATERIALIZED (
                                    SELECT S."SubmissionId",
                                           S."TimeSubmitted",
                                           S."MonitoringObserverId",
                                           S."DisplayName",
                                           S."Email",
                                           S."PhoneNumber",
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

                                (SELECT COALESCE(SUM("NumberOfQuestionsAnswered"), 0) FROM SUBMISSIONS)::INT AS questions_answered,

                                (SELECT COALESCE(SUM("NumberOfFlaggedAnswers"), 0) FROM SUBMISSIONS)::INT AS flagged_answers,

                                (SELECT COUNT(DISTINCT "PollingStationId") FROM SUBMISSIONS)::INT AS polling_stations,

                                (SELECT COALESCE(JSONB_AGG(DISTINCT JSONB_BUILD_OBJECT(
                                        'ResponderId', "MonitoringObserverId",
                                        'DisplayName', "DisplayName",
                                        'Email', "Email",
                                        'PhoneNumber', "PhoneNumber")), '[]'::JSONB)
                                 FROM SUBMISSIONS) AS responders,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', "SubmissionId",
                                        'ResponderId', "MonitoringObserverId",
                                        'TimeSubmitted', "TimeSubmitted",
                                        'Answers', "Answers",
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
                                          CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Notes") N) AS notes,

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
                                          CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(S."Attachments") F) AS attachments
                            $sql$, filter)
                    INTO v_collected
                    USING filterParameters, electionRoundId, ngoId, dataSource, formId;

                    v_aggregates := "AggregateAnswers"(v_form.questions, v_collected.submissions);
                    v_texts := "ExtractTexts"(v_aggregates);
                    v_notes := "NumberItems"(v_collected.notes);
                    v_attachments := "NumberItems"(v_collected.attachments);
                    v_locations := "AggregateAnswersByLocation"(v_form.questions, v_collected.submissions, v_texts -> 'Ids', v_notes, v_attachments);

                    RETURN QUERY
                        SELECT v_form.id,
                               v_form.code,
                               v_form.form_type,
                               v_form.name,
                               v_form.description,
                               v_form.default_language,
                               v_form.languages,
                               v_collected.responders,
                               v_collected.submission_count,
                               v_collected.questions_answered,
                               v_collected.flagged_answers,
                               v_collected.polling_stations,
                               "AttachLocationAggregates"("ReferenceAnswers"(v_aggregates, v_texts -> 'Ids', v_notes, v_attachments, TRUE), v_locations -> 'ByQuestion'),
                               v_notes,
                               v_attachments,
                               v_texts -> 'Texts',
                               v_locations -> 'Locations';
                END;
                $$ LANGUAGE plpgsql;
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
        }
    }
}
