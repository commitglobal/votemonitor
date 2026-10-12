using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionsAggregateFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                -- Builds the answer aggregates of a form, keyed by question id, shaped like the BaseAnswerAggregate subtypes.
                -- p_questions: the form's questions as stored.
                -- p_submissions: [{ "submissionId", "responderId", "timeSubmitted", "answers" }] of the submissions to aggregate.
                CREATE OR REPLACE FUNCTION "AggregateAnswers"(
                    p_questions JSONB,
                    p_submissions JSONB
                )
                    RETURNS JSONB
                AS
                $$
                DECLARE
                    v_aggregates JSONB;
                    v_collected  RECORD;
                BEGIN
                    -- 1. an empty aggregate for every question, depending on its type
                    SELECT COALESCE(JSONB_OBJECT_AGG(
                                            Q.question ->> 'Id',
                                            JSONB_BUILD_OBJECT(
                                                    '$questionType', REPLACE(Q.question ->> '$questionType', 'Question', 'AnswerAggregate'),
                                                    'QuestionId', Q.question -> 'Id',
                                                    'DisplayOrder', Q.idx - 1,
                                                    'Question', Q.question,
                                                    'AnswersAggregated', 0
                                            ) || CASE
                                                     WHEN Q.question ->> '$questionType' = 'textQuestion'
                                                         THEN JSONB_BUILD_OBJECT('Answers', '[]'::JSONB)
                                                     WHEN Q.question ->> '$questionType' = 'numberQuestion'
                                                         THEN JSONB_BUILD_OBJECT('Min', 2147483647, 'Max', -2147483648, 'Average', 0)
                                                     WHEN Q.question ->> '$questionType' = 'dateQuestion'
                                                         THEN JSONB_BUILD_OBJECT('AnswersHistogram', '{}'::JSONB)
                                                     WHEN Q.question ->> '$questionType' = 'ratingQuestion'
                                                         THEN JSONB_BUILD_OBJECT(
                                                             'Min', 2147483647, 'Max', -2147483648, 'Average', 0,
                                                             'AnswersHistogram', (
                                                                 SELECT JSONB_OBJECT_AGG(V::TEXT, 0)
                                                                 FROM GENERATE_SERIES(1, SUBSTRING(Q.question ->> 'Scale' FROM 'OneTo(\d+)')::INT) V))
                                                     WHEN Q.question ->> '$questionType' IN ('singleSelectQuestion', 'multiSelectQuestion')
                                                         THEN JSONB_BUILD_OBJECT(
                                                             'AnswersHistogram', (
                                                                 SELECT COALESCE(JSONB_OBJECT_AGG(O ->> 'Id', 0), '{}'::JSONB)
                                                                 FROM JSONB_ARRAY_ELEMENTS(Q.question -> 'Options') O),
                                                             'FreeTexts', '[]'::JSONB)
                                                     ELSE '{}'::JSONB
                                            END
                                    ), '{}'::JSONB)
                    INTO v_aggregates
                    FROM JSONB_ARRAY_ELEMENTS(p_questions) WITH ORDINALITY Q(question, idx);

                    -- 2. collect data from the submissions' answers
                    WITH
                        SUBMISSION_ANSWERS AS (
                            SELECT (S ->> 'SubmissionId')::UUID         AS submission_id,
                                   (S ->> 'ResponderId')::UUID          AS responder_id,
                                   (S ->> 'TimeSubmitted')::TIMESTAMPTZ AS time_submitted,
                                   S -> 'Answers'                       AS answers
                            FROM JSONB_ARRAY_ELEMENTS(p_submissions) S
                        ),
                        FORM_QUESTIONS AS (
                            SELECT Q ->> 'Id'                                            AS question_id,
                                   REPLACE(Q ->> '$questionType', 'Question', 'Answer') AS answer_type,
                                   Q -> 'Options'                                        AS options
                            FROM JSONB_ARRAY_ELEMENTS(p_questions) Q
                        ),
                        -- answers to the form's questions, of the type each question expects
                        ANSWERS AS (
                            SELECT SA.submission_id, SA.responder_id, SA.time_submitted,
                                   FQ.question_id, FQ.answer_type, FQ.options, A AS answer
                            FROM SUBMISSION_ANSWERS SA
                                     CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(SA.answers) A
                                     INNER JOIN FORM_QUESTIONS FQ ON FQ.question_id = A ->> 'QuestionId'
                                AND FQ.answer_type = A ->> '$answerType'
                        ),
                        -- one row per selected option of single and multi select answers
                        SELECTIONS AS (
                            SELECT AN.*, SEL AS selection
                            FROM ANSWERS AN
                                     CROSS JOIN LATERAL JSONB_ARRAY_ELEMENTS(
                                    CASE
                                        WHEN AN.answer_type = 'singleSelectAnswer' THEN JSONB_BUILD_ARRAY(AN.answer -> 'Selection')
                                        ELSE AN.answer -> 'Selection'
                                    END) SEL
                            WHERE AN.answer_type IN ('singleSelectAnswer', 'multiSelectAnswer')
                        )
                    SELECT
                        -- per question: number of answers, and min/max/average for number and rating answers
                        (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, stats), '{}'::JSONB)
                         FROM (SELECT question_id,
                                      JSONB_BUILD_OBJECT('AnswersAggregated', COUNT(*)) ||
                                      CASE
                                          WHEN answer_type IN ('numberAnswer', 'ratingAnswer') THEN JSONB_BUILD_OBJECT(
                                                  'Min', MIN((answer ->> 'Value')::INT),
                                                  'Max', MAX((answer ->> 'Value')::INT),
                                                  'Average', AVG((answer ->> 'Value')::INT))
                                          ELSE '{}'::JSONB
                                      END AS stats
                               FROM ANSWERS
                               GROUP BY question_id, answer_type) X) AS stats,

                        -- per question: rating values, hour buckets of dates and selected options with their counts
                        (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, histogram), '{}'::JSONB)
                         FROM (SELECT question_id, JSONB_OBJECT_AGG(KEY, count) AS histogram
                               FROM (SELECT question_id, answer ->> 'Value' AS key, COUNT(*) AS count
                                     FROM ANSWERS
                                     WHERE answer_type = 'ratingAnswer'
                                     GROUP BY 1, 2
                                     UNION ALL
                                     SELECT question_id,
                                            TO_CHAR(DATE_TRUNC('hour', (answer ->> 'Date')::TIMESTAMPTZ AT TIME ZONE 'UTC'),
                                                    'YYYY-MM-DD"T"HH24":00:00.0000000Z"'),
                                            COUNT(*)
                                     FROM ANSWERS
                                     WHERE answer_type = 'dateAnswer'
                                     GROUP BY 1, 2
                                     UNION ALL
                                     SELECT question_id, selection ->> 'OptionId', COUNT(*)
                                     FROM SELECTIONS
                                     WHERE selection ->> 'OptionId' IS NOT NULL
                                     GROUP BY 1, 2) H
                               GROUP BY question_id) X) AS histograms,

                        -- per question: text answers, and free texts typed for free text options
                        (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, texts), '{}'::JSONB)
                         FROM (SELECT question_id,
                                      JSONB_AGG(JSONB_BUILD_OBJECT(
                                              'SubmissionId', submission_id,
                                              'ResponderId', responder_id,
                                              'Value', text) ORDER BY time_submitted) AS texts
                               FROM (SELECT question_id, submission_id, responder_id, time_submitted, answer ->> 'Text' AS text
                                     FROM ANSWERS
                                     WHERE answer_type = 'textAnswer'
                                     UNION ALL
                                     SELECT SL.question_id, SL.submission_id, SL.responder_id, SL.time_submitted,
                                            SL.selection ->> 'Text'
                                     FROM SELECTIONS SL
                                     WHERE NULLIF(BTRIM(SL.selection ->> 'Text'), '') IS NOT NULL
                                       AND EXISTS (SELECT 1
                                                   FROM JSONB_ARRAY_ELEMENTS(SL.options) O
                                                   WHERE O ->> 'Id' = SL.selection ->> 'OptionId'
                                                     AND (O ->> 'IsFreeText')::BOOLEAN)) T
                               GROUP BY question_id) X) AS texts
                    INTO v_collected;

                    -- 3. merge the collected data into the empty aggregates
                    RETURN (SELECT COALESCE(JSONB_OBJECT_AGG(
                                                    E.key,
                                                    E.value
                                                        || COALESCE(v_collected.stats -> E.key, '{}'::JSONB)
                                                        || CASE
                                                               WHEN E.value ->> '$questionType' = 'textAnswerAggregate'
                                                                   THEN JSONB_BUILD_OBJECT('Answers', COALESCE(v_collected.texts -> E.key, '[]'::JSONB))
                                                               WHEN E.value ->> '$questionType' IN ('singleSelectAnswerAggregate', 'multiSelectAnswerAggregate')
                                                                   THEN JSONB_BUILD_OBJECT(
                                                                       'AnswersHistogram', (E.value -> 'AnswersHistogram') || COALESCE(v_collected.histograms -> E.key, '{}'::JSONB),
                                                                       'FreeTexts', COALESCE(v_collected.texts -> E.key, '[]'::JSONB))
                                                               WHEN E.value ->> '$questionType' IN ('ratingAnswerAggregate', 'dateAnswerAggregate')
                                                                   THEN JSONB_BUILD_OBJECT(
                                                                       'AnswersHistogram', (E.value -> 'AnswersHistogram') || COALESCE(v_collected.histograms -> E.key, '{}'::JSONB))
                                                               ELSE '{}'::JSONB
                                                        END
                                            ), '{}'::JSONB)
                            FROM JSONB_EACH(v_aggregates) E);
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                -- Aggregates the answers of a form's submissions. Returns one row shaped like FormSubmissionsAggregate,
                -- or no rows when the form is not available to the ngo.
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
                                "Attachments"                    JSONB
                            )
                AS
                $$
                #variable_conflict use_column
                DECLARE
                    v_form      RECORD;
                    v_collected RECORD;
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
                                        'Answers', "Answers")), '[]'::JSONB)
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
                               "AggregateAnswers"(v_form.questions, v_collected.submissions),
                               v_collected.notes,
                               v_collected.attachments;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                -- Aggregates the answers of an incident reporting form. Returns one row shaped like FormSubmissionsAggregate,
                -- or no rows when the form does not belong to the ngo.
                --
                -- filter is a SQL predicate over "GetIncidentReportEntries" aliased as s, built server-side by FilterSqlCompiler.
                -- Its values are read from filterParameters as ($1 ->> 'name'), so they are never part of the SQL text.
                CREATE OR REPLACE FUNCTION "GetIncidentReportsAggregate"(
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
                                "Attachments"                    JSONB
                            )
                AS
                $$
                #variable_conflict use_column
                DECLARE
                    v_form      RECORD;
                    v_collected RECORD;
                BEGIN
                    -- load the form: a form of the ngo in this election round
                    SELECT F."Id"                    AS id,
                           F."Code"::TEXT            AS code,
                           F."FormType"              AS form_type,
                           F."Name"                  AS name,
                           F."Description"           AS description,
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

                    -- load the incident reports matching the filter, with their notes and attachments
                    EXECUTE FORMAT(
                            $sql$
                            WITH
                                SUBMISSIONS AS MATERIALIZED (
                                    SELECT S."Id" AS "SubmissionId",
                                           S."TimeSubmitted",
                                           S."MonitoringObserverId",
                                           S."DisplayName",
                                           S."Email",
                                           S."PhoneNumber",
                                           S."PollingStationId",
                                           S."NumberOfQuestionsAnswered",
                                           S."NumberOfFlaggedAnswers",
                                           S."Answers"
                                    FROM "GetIncidentReportEntries"($2, $3, $4) S
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
                                        'Answers', "Answers")), '[]'::JSONB)
                                 FROM SUBMISSIONS) AS submissions,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'QuestionId', N."QuestionId",
                                        'SubmissionId', S."SubmissionId",
                                        'Text', N."Text",
                                        'TimeSubmitted', COALESCE(N."LastModifiedOn", N."CreatedOn"),
                                        'MonitoringObserverId', S."MonitoringObserverId")), '[]'::JSONB)
                                 FROM SUBMISSIONS S
                                          INNER JOIN "IncidentReportNotes" N ON N."IncidentReportId" = S."SubmissionId") AS notes,

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
                                 WHERE A."IsDeleted" = FALSE
                                   AND A."IsCompleted" = TRUE) AS attachments
                            $sql$, filter)
                    INTO v_collected
                    USING filterParameters, electionRoundId, ngoId, dataSource, formId;

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
                               "AggregateAnswers"(v_form.questions, v_collected.submissions),
                               v_collected.notes,
                               v_collected.attachments;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                -- Aggregates the answers of a citizen reporting form. Returns one row shaped like CitizenReportFormSubmissionsAggregate,
                -- or no rows when the form does not belong to the ngo. Citizen reports have no observer, so responder ids are empty.
                --
                -- filter is a SQL predicate over "GetCitizenReportEntries" aliased as s, built server-side by FilterSqlCompiler.
                -- Its values are read from filterParameters as ($1 ->> 'name'), so they are never part of the SQL text.
                CREATE OR REPLACE FUNCTION "GetCitizenReportsAggregate"(
                    electionRoundId UUID,
                    ngoId UUID,
                    formId UUID,
                    filter TEXT DEFAULT 'TRUE',
                    filterParameters JSONB DEFAULT '{}'::JSONB
                )
                    RETURNS TABLE
                            (
                                "ElectionRoundId"                UUID,
                                "MonitoringNgoId"                UUID,
                                "FormId"                         UUID,
                                "FormCode"                       TEXT,
                                "FormType"                       TEXT,
                                "Name"                           JSONB,
                                "Description"                    JSONB,
                                "DefaultLanguage"                TEXT,
                                "Languages"                      TEXT[],
                                "SubmissionCount"                INTEGER,
                                "TotalNumberOfQuestionsAnswered" INTEGER,
                                "TotalNumberOfFlaggedAnswers"    INTEGER,
                                "Aggregates"                     JSONB,
                                "Notes"                          JSONB,
                                "Attachments"                    JSONB
                            )
                AS
                $$
                #variable_conflict use_column
                DECLARE
                    v_form      RECORD;
                    v_collected RECORD;
                BEGIN
                    -- load the form: a form of the ngo in this election round
                    SELECT F."Id"                    AS id,
                           F."ElectionRoundId"       AS election_round_id,
                           F."MonitoringNgoId"       AS monitoring_ngo_id,
                           F."Code"::TEXT            AS code,
                           F."FormType"              AS form_type,
                           F."Name"                  AS name,
                           F."Description"           AS description,
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

                    -- load the citizen reports matching the filter, with their notes and attachments
                    EXECUTE FORMAT(
                            $sql$
                            WITH
                                SUBMISSIONS AS MATERIALIZED (
                                    SELECT S."Id" AS "SubmissionId",
                                           S."TimeSubmitted",
                                           S."NumberOfQuestionsAnswered",
                                           S."NumberOfFlaggedAnswers",
                                           S."Answers"
                                    FROM "GetCitizenReportEntries"($2, $3) S
                                    WHERE S."FormId" = $4
                                      AND (%s)
                                )
                            SELECT
                                (SELECT COUNT(*) FROM SUBMISSIONS)::INT AS submission_count,

                                (SELECT COALESCE(SUM("NumberOfQuestionsAnswered"), 0) FROM SUBMISSIONS)::INT AS questions_answered,

                                (SELECT COALESCE(SUM("NumberOfFlaggedAnswers"), 0) FROM SUBMISSIONS)::INT AS flagged_answers,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', "SubmissionId",
                                        'ResponderId', '00000000-0000-0000-0000-000000000000'::UUID,
                                        'TimeSubmitted', "TimeSubmitted",
                                        'Answers', "Answers")), '[]'::JSONB)
                                 FROM SUBMISSIONS) AS submissions,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'QuestionId', N."QuestionId",
                                        'SubmissionId', S."SubmissionId",
                                        'Text', N."Text",
                                        'TimeSubmitted', COALESCE(N."LastModifiedOn", N."CreatedOn"))), '[]'::JSONB)
                                 FROM SUBMISSIONS S
                                          INNER JOIN "CitizenReportNotes" N ON N."CitizenReportId" = S."SubmissionId") AS notes,

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
                                 WHERE A."IsDeleted" = FALSE
                                   AND A."IsCompleted" = TRUE) AS attachments
                            $sql$, filter)
                    INTO v_collected
                    USING filterParameters, electionRoundId, ngoId, formId;

                    RETURN QUERY
                        SELECT v_form.election_round_id,
                               v_form.monitoring_ngo_id,
                               v_form.id,
                               v_form.code,
                               v_form.form_type,
                               v_form.name,
                               v_form.description,
                               v_form.default_language,
                               v_form.languages,
                               v_collected.submission_count,
                               v_collected.questions_answered,
                               v_collected.flagged_answers,
                               "AggregateAnswers"(v_form.questions, v_collected.submissions),
                               v_collected.notes,
                               v_collected.attachments;
                END;
                $$ LANGUAGE plpgsql;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportsAggregate"(uuid, uuid, uuid, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportsAggregate"(uuid, uuid, text, uuid, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionsAggregate"(uuid, uuid, text, uuid, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "AggregateAnswers"(jsonb, jsonb);""");
        }
    }
}
