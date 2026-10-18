using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddCitizenReportLocationsCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                -- Builds the answer aggregates of a form, keyed by question id, shaped like the BaseAnswerAggregate subtypes.
                -- p_questions: the form's questions as stored.
                -- p_submissions: [{ "submissionId", "responderId", "timeSubmitted", "answers", "pollingStationId" | "locationId" }] of the submissions to aggregate.
                -- Single select, multi select and rating aggregates also count, per option / value, the distinct submissions
                -- ("SubmissionsHistogram"), polling stations ("PollingStationsHistogram") and citizen report locations ("LocationsHistogram")
                -- that picked it. The polling stations histogram is only built when the submissions carry "pollingStationId" (not for
                -- citizen reports), the locations histogram only when they carry "locationId" (only citizen reports).
                CREATE OR REPLACE FUNCTION "AggregateAnswers"(
                    p_questions JSONB,
                    p_submissions JSONB
                )
                    RETURNS JSONB
                AS
                $$
                DECLARE
                    v_aggregates            JSONB;
                    v_collected             RECORD;
                    v_has_polling_stations  BOOLEAN;
                    v_has_locations         BOOLEAN;
                BEGIN
                    SELECT COALESCE(BOOL_OR(S ? 'PollingStationId'), FALSE),
                           COALESCE(BOOL_OR(S ? 'LocationId'), FALSE)
                    INTO v_has_polling_stations, v_has_locations
                    FROM JSONB_ARRAY_ELEMENTS(p_submissions) S;

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
                                                     WHEN Q.question ->> '$questionType' IN ('dateQuestion', 'timeQuestion', 'dateTimeQuestion')
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
                                   (S ->> 'PollingStationId')::UUID     AS polling_station_id,
                                   (S ->> 'LocationId')::UUID           AS location_id,
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
                            SELECT SA.submission_id, SA.responder_id, SA.time_submitted, SA.polling_station_id, SA.location_id,
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
                        ),
                        -- the rating value or selected option picked in each answer, for counting submissions, polling stations and locations
                        PICKS AS (
                            SELECT question_id, answer ->> 'Value' AS key, submission_id, polling_station_id, location_id
                            FROM ANSWERS
                            WHERE answer_type = 'ratingAnswer'
                            UNION ALL
                            SELECT question_id, selection ->> 'OptionId', submission_id, polling_station_id, location_id
                            FROM SELECTIONS
                            WHERE selection ->> 'OptionId' IS NOT NULL
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

                        -- per question: rating values, hour buckets of dates and date times, minute buckets of times and selected options with their counts
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
                                     SELECT question_id,
                                            TO_CHAR(DATE_TRUNC('hour', (answer ->> 'DateTime')::TIMESTAMPTZ AT TIME ZONE 'UTC'),
                                                    'YYYY-MM-DD"T"HH24":00:00.0000000Z"'),
                                            COUNT(*)
                                     FROM ANSWERS
                                     WHERE answer_type = 'dateTimeAnswer'
                                     GROUP BY 1, 2
                                     UNION ALL
                                     SELECT question_id, TO_CHAR((answer ->> 'Time')::TIME, 'HH24:MI'), COUNT(*)
                                     FROM ANSWERS
                                     WHERE answer_type = 'timeAnswer'
                                     GROUP BY 1, 2
                                     UNION ALL
                                     SELECT question_id, selection ->> 'OptionId', COUNT(*)
                                     FROM SELECTIONS
                                     WHERE selection ->> 'OptionId' IS NOT NULL
                                     GROUP BY 1, 2) H
                               GROUP BY question_id) X) AS histograms,

                        -- per question: distinct submissions per rating value / selected option
                        (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, histogram), '{}'::JSONB)
                         FROM (SELECT question_id, JSONB_OBJECT_AGG(key, count) AS histogram
                               FROM (SELECT question_id, key, COUNT(DISTINCT submission_id) AS count
                                     FROM PICKS
                                     GROUP BY 1, 2) H
                               GROUP BY question_id) X) AS submission_histograms,

                        -- per question: distinct polling stations per rating value / selected option
                        (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, histogram), '{}'::JSONB)
                         FROM (SELECT question_id, JSONB_OBJECT_AGG(key, count) AS histogram
                               FROM (SELECT question_id, key, COUNT(DISTINCT polling_station_id) AS count
                                     FROM PICKS
                                     GROUP BY 1, 2) H
                               GROUP BY question_id) X) AS polling_station_histograms,

                        -- per question: distinct citizen report locations per rating value / selected option
                        (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, histogram), '{}'::JSONB)
                         FROM (SELECT question_id, JSONB_OBJECT_AGG(key, count) AS histogram
                               FROM (SELECT question_id, key, COUNT(DISTINCT location_id) AS count
                                     FROM PICKS
                                     GROUP BY 1, 2) H
                               GROUP BY question_id) X) AS location_histograms,

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
                                                               WHEN E.value ->> '$questionType' IN ('ratingAnswerAggregate', 'dateAnswerAggregate', 'timeAnswerAggregate', 'dateTimeAnswerAggregate')
                                                                   THEN JSONB_BUILD_OBJECT(
                                                                       'AnswersHistogram', (E.value -> 'AnswersHistogram') || COALESCE(v_collected.histograms -> E.key, '{}'::JSONB))
                                                               ELSE '{}'::JSONB
                                                        END
                                                        || CASE
                                                               WHEN E.value ->> '$questionType' IN ('singleSelectAnswerAggregate', 'multiSelectAnswerAggregate', 'ratingAnswerAggregate')
                                                                   THEN JSONB_BUILD_OBJECT(
                                                                       'SubmissionsHistogram', Z.zeros || COALESCE(v_collected.submission_histograms -> E.key, '{}'::JSONB))
                                                                   || CASE
                                                                          WHEN v_has_polling_stations THEN JSONB_BUILD_OBJECT(
                                                                              'PollingStationsHistogram', Z.zeros || COALESCE(v_collected.polling_station_histograms -> E.key, '{}'::JSONB))
                                                                          ELSE '{}'::JSONB
                                                                      END
                                                                   || CASE
                                                                          WHEN v_has_locations THEN JSONB_BUILD_OBJECT(
                                                                              'LocationsHistogram', Z.zeros || COALESCE(v_collected.location_histograms -> E.key, '{}'::JSONB))
                                                                          ELSE '{}'::JSONB
                                                                      END
                                                               ELSE '{}'::JSONB
                                                        END
                                            ), '{}'::JSONB)
                            FROM JSONB_EACH(v_aggregates) E
                                     -- every option / rating value with a count of 0
                                     CROSS JOIN LATERAL (
                                SELECT COALESCE(JSONB_OBJECT_AGG(K, 0), '{}'::JSONB) AS zeros
                                FROM JSONB_OBJECT_KEYS(CASE WHEN JSONB_TYPEOF(E.value -> 'AnswersHistogram') = 'object'
                                                            THEN E.value -> 'AnswersHistogram'
                                                            ELSE '{}'::JSONB END) K
                                ) Z);
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                -- Builds the entry of one location group: its submission count, totals and answer aggregates (see "ReferenceAnswers"),
                -- referencing only the notes and attachments of the group's submissions.
                -- p_submissions: the group's submissions, shaped as for "AggregateAnswersByLocation".
                CREATE OR REPLACE FUNCTION "AggregateLocationGroup"(
                    p_questions JSONB,
                    p_submissions JSONB,
                    p_text_ids JSONB,
                    p_notes JSONB,
                    p_attachments JSONB
                )
                    RETURNS JSONB
                AS
                $$
                WITH
                    SUBMISSION_IDS AS (
                        SELECT S ->> 'SubmissionId' AS submission_id
                        FROM JSONB_ARRAY_ELEMENTS(p_submissions) S
                    )
                SELECT JSONB_BUILD_OBJECT(
                               'SubmissionCount', JSONB_ARRAY_LENGTH(p_submissions),
                               'TotalNumberOfQuestionsAnswered', (SELECT COALESCE(SUM((S ->> 'NumberOfQuestionsAnswered')::INT), 0)
                                                                  FROM JSONB_ARRAY_ELEMENTS(p_submissions) S),
                               'TotalNumberOfFlaggedAnswers', (SELECT COALESCE(SUM((S ->> 'NumberOfFlaggedAnswers')::INT), 0)
                                                               FROM JSONB_ARRAY_ELEMENTS(p_submissions) S),
                               -- only for submissions made at polling stations (not citizen reports)
                               'PollingStationsCount', (SELECT CASE WHEN BOOL_OR(S ? 'PollingStationId')
                                                                    THEN COUNT(DISTINCT S ->> 'PollingStationId') END
                                                        FROM JSONB_ARRAY_ELEMENTS(p_submissions) S),
                               -- only for citizen reports
                               'LocationsCount', (SELECT CASE WHEN BOOL_OR(S ? 'LocationId')
                                                              THEN COUNT(DISTINCT S ->> 'LocationId') END
                                                  FROM JSONB_ARRAY_ELEMENTS(p_submissions) S),
                               'Aggregates', "ReferenceAnswers"(
                                       "AggregateAnswers"(p_questions, p_submissions),
                                       p_text_ids,
                                       (SELECT COALESCE(JSONB_AGG(N), '[]'::JSONB)
                                        FROM JSONB_ARRAY_ELEMENTS(p_notes) N
                                        WHERE N ->> 'SubmissionId' IN (SELECT submission_id FROM SUBMISSION_IDS)),
                                       (SELECT COALESCE(JSONB_AGG(F), '[]'::JSONB)
                                        FROM JSONB_ARRAY_ELEMENTS(p_attachments) F
                                        WHERE F ->> 'SubmissionId' IN (SELECT submission_id FROM SUBMISSION_IDS)),
                                       FALSE)
                       );
                $$ LANGUAGE sql STABLE;
                """);

            // "GetCitizenReportsAggregate" returns a different set of columns, so it has to be dropped and recreated
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportsAggregate"(uuid, uuid, uuid, text, jsonb);""");

            migrationBuilder.Sql(
                """
                -- Aggregates the answers of a citizen reporting form. Returns one row shaped like CitizenReportFormSubmissionsAggregate,
                -- or no rows when the form does not belong to the ngo. Citizen reports have no observer, so responder ids are empty.
                -- Texts, Notes and Attachments are numbered ("Id") and the question aggregates reference them by id
                -- (see "ReferenceAnswers"). Locations lists the Level1..Level5 paths of the submissions and every question
                -- aggregate holds its breakdown per location id in "LocationAggregates" (see "AggregateAnswersByLocation").
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
                                "LocationsCount"                 INTEGER,
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

                                (SELECT COALESCE(SUM("NumberOfQuestionsAnswered"), 0) FROM SUBMISSIONS)::INT AS questions_answered,

                                (SELECT COALESCE(SUM("NumberOfFlaggedAnswers"), 0) FROM SUBMISSIONS)::INT AS flagged_answers,

                                (SELECT COUNT(DISTINCT "LocationId") FROM SUBMISSIONS)::INT AS locations,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', "SubmissionId",
                                        'ResponderId', '00000000-0000-0000-0000-000000000000'::UUID,
                                        'TimeSubmitted', "TimeSubmitted",
                                        'Answers', "Answers",
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

                    v_aggregates := "AggregateAnswers"(v_form.questions, v_collected.submissions);
                    v_texts := "ExtractTexts"(v_aggregates);
                    v_notes := "NumberItems"(v_collected.notes);
                    v_attachments := "NumberItems"(v_collected.attachments);
                    v_locations := "AggregateAnswersByLocation"(v_form.questions, v_collected.submissions, v_texts -> 'Ids', v_notes, v_attachments);

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
                               v_collected.locations,
                               "AttachLocationAggregates"("ReferenceAnswers"(v_aggregates, v_texts -> 'Ids', v_notes, v_attachments, TRUE), v_locations -> 'ByQuestion'),
                               v_notes,
                               v_attachments,
                               v_texts -> 'Texts',
                               v_locations -> 'Locations';
                END;
                $$ LANGUAGE plpgsql;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                -- Builds the answer aggregates of a form, keyed by question id, shaped like the BaseAnswerAggregate subtypes.
                -- p_questions: the form's questions as stored.
                -- p_submissions: [{ "submissionId", "responderId", "timeSubmitted", "answers", "pollingStationId" }] of the submissions to aggregate.
                -- Single select, multi select and rating aggregates also count, per option / value, the distinct submissions
                -- ("SubmissionsHistogram") and polling stations ("PollingStationsHistogram") that picked it. The polling stations
                -- histogram is only built when the submissions carry "pollingStationId" (not for citizen reports).
                CREATE OR REPLACE FUNCTION "AggregateAnswers"(
                    p_questions JSONB,
                    p_submissions JSONB
                )
                    RETURNS JSONB
                AS
                $$
                DECLARE
                    v_aggregates            JSONB;
                    v_collected             RECORD;
                    v_has_polling_stations  BOOLEAN;
                BEGIN
                    SELECT COALESCE(BOOL_OR(S ? 'PollingStationId'), FALSE)
                    INTO v_has_polling_stations
                    FROM JSONB_ARRAY_ELEMENTS(p_submissions) S;

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
                                                     WHEN Q.question ->> '$questionType' IN ('dateQuestion', 'timeQuestion', 'dateTimeQuestion')
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
                                   (S ->> 'PollingStationId')::UUID     AS polling_station_id,
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
                            SELECT SA.submission_id, SA.responder_id, SA.time_submitted, SA.polling_station_id,
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
                        ),
                        -- the rating value or selected option picked in each answer, for counting submissions and polling stations
                        PICKS AS (
                            SELECT question_id, answer ->> 'Value' AS key, submission_id, polling_station_id
                            FROM ANSWERS
                            WHERE answer_type = 'ratingAnswer'
                            UNION ALL
                            SELECT question_id, selection ->> 'OptionId', submission_id, polling_station_id
                            FROM SELECTIONS
                            WHERE selection ->> 'OptionId' IS NOT NULL
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

                        -- per question: rating values, hour buckets of dates and date times, minute buckets of times and selected options with their counts
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
                                     SELECT question_id,
                                            TO_CHAR(DATE_TRUNC('hour', (answer ->> 'DateTime')::TIMESTAMPTZ AT TIME ZONE 'UTC'),
                                                    'YYYY-MM-DD"T"HH24":00:00.0000000Z"'),
                                            COUNT(*)
                                     FROM ANSWERS
                                     WHERE answer_type = 'dateTimeAnswer'
                                     GROUP BY 1, 2
                                     UNION ALL
                                     SELECT question_id, TO_CHAR((answer ->> 'Time')::TIME, 'HH24:MI'), COUNT(*)
                                     FROM ANSWERS
                                     WHERE answer_type = 'timeAnswer'
                                     GROUP BY 1, 2
                                     UNION ALL
                                     SELECT question_id, selection ->> 'OptionId', COUNT(*)
                                     FROM SELECTIONS
                                     WHERE selection ->> 'OptionId' IS NOT NULL
                                     GROUP BY 1, 2) H
                               GROUP BY question_id) X) AS histograms,

                        -- per question: distinct submissions per rating value / selected option
                        (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, histogram), '{}'::JSONB)
                         FROM (SELECT question_id, JSONB_OBJECT_AGG(key, count) AS histogram
                               FROM (SELECT question_id, key, COUNT(DISTINCT submission_id) AS count
                                     FROM PICKS
                                     GROUP BY 1, 2) H
                               GROUP BY question_id) X) AS submission_histograms,

                        -- per question: distinct polling stations per rating value / selected option
                        (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, histogram), '{}'::JSONB)
                         FROM (SELECT question_id, JSONB_OBJECT_AGG(key, count) AS histogram
                               FROM (SELECT question_id, key, COUNT(DISTINCT polling_station_id) AS count
                                     FROM PICKS
                                     GROUP BY 1, 2) H
                               GROUP BY question_id) X) AS polling_station_histograms,

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
                                                               WHEN E.value ->> '$questionType' IN ('ratingAnswerAggregate', 'dateAnswerAggregate', 'timeAnswerAggregate', 'dateTimeAnswerAggregate')
                                                                   THEN JSONB_BUILD_OBJECT(
                                                                       'AnswersHistogram', (E.value -> 'AnswersHistogram') || COALESCE(v_collected.histograms -> E.key, '{}'::JSONB))
                                                               ELSE '{}'::JSONB
                                                        END
                                                        || CASE
                                                               WHEN E.value ->> '$questionType' IN ('singleSelectAnswerAggregate', 'multiSelectAnswerAggregate', 'ratingAnswerAggregate')
                                                                   THEN JSONB_BUILD_OBJECT(
                                                                       'SubmissionsHistogram', Z.zeros || COALESCE(v_collected.submission_histograms -> E.key, '{}'::JSONB))
                                                                   || CASE
                                                                          WHEN v_has_polling_stations THEN JSONB_BUILD_OBJECT(
                                                                              'PollingStationsHistogram', Z.zeros || COALESCE(v_collected.polling_station_histograms -> E.key, '{}'::JSONB))
                                                                          ELSE '{}'::JSONB
                                                                      END
                                                               ELSE '{}'::JSONB
                                                        END
                                            ), '{}'::JSONB)
                            FROM JSONB_EACH(v_aggregates) E
                                     -- every option / rating value with a count of 0
                                     CROSS JOIN LATERAL (
                                SELECT COALESCE(JSONB_OBJECT_AGG(K, 0), '{}'::JSONB) AS zeros
                                FROM JSONB_OBJECT_KEYS(CASE WHEN JSONB_TYPEOF(E.value -> 'AnswersHistogram') = 'object'
                                                            THEN E.value -> 'AnswersHistogram'
                                                            ELSE '{}'::JSONB END) K
                                ) Z);
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                -- Builds the entry of one location group: its submission count, totals and answer aggregates (see "ReferenceAnswers"),
                -- referencing only the notes and attachments of the group's submissions.
                -- p_submissions: the group's submissions, shaped as for "AggregateAnswersByLocation".
                CREATE OR REPLACE FUNCTION "AggregateLocationGroup"(
                    p_questions JSONB,
                    p_submissions JSONB,
                    p_text_ids JSONB,
                    p_notes JSONB,
                    p_attachments JSONB
                )
                    RETURNS JSONB
                AS
                $$
                WITH
                    SUBMISSION_IDS AS (
                        SELECT S ->> 'SubmissionId' AS submission_id
                        FROM JSONB_ARRAY_ELEMENTS(p_submissions) S
                    )
                SELECT JSONB_BUILD_OBJECT(
                               'SubmissionCount', JSONB_ARRAY_LENGTH(p_submissions),
                               'TotalNumberOfQuestionsAnswered', (SELECT COALESCE(SUM((S ->> 'NumberOfQuestionsAnswered')::INT), 0)
                                                                  FROM JSONB_ARRAY_ELEMENTS(p_submissions) S),
                               'TotalNumberOfFlaggedAnswers', (SELECT COALESCE(SUM((S ->> 'NumberOfFlaggedAnswers')::INT), 0)
                                                               FROM JSONB_ARRAY_ELEMENTS(p_submissions) S),
                               -- only for submissions made at polling stations (not citizen reports)
                               'PollingStationsCount', (SELECT CASE WHEN BOOL_OR(S ? 'PollingStationId')
                                                                    THEN COUNT(DISTINCT S ->> 'PollingStationId') END
                                                        FROM JSONB_ARRAY_ELEMENTS(p_submissions) S),
                               'Aggregates', "ReferenceAnswers"(
                                       "AggregateAnswers"(p_questions, p_submissions),
                                       p_text_ids,
                                       (SELECT COALESCE(JSONB_AGG(N), '[]'::JSONB)
                                        FROM JSONB_ARRAY_ELEMENTS(p_notes) N
                                        WHERE N ->> 'SubmissionId' IN (SELECT submission_id FROM SUBMISSION_IDS)),
                                       (SELECT COALESCE(JSONB_AGG(F), '[]'::JSONB)
                                        FROM JSONB_ARRAY_ELEMENTS(p_attachments) F
                                        WHERE F ->> 'SubmissionId' IN (SELECT submission_id FROM SUBMISSION_IDS)),
                                       FALSE)
                       );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportsAggregate"(uuid, uuid, uuid, text, jsonb);""");

            migrationBuilder.Sql(
                """
                -- Aggregates the answers of a citizen reporting form. Returns one row shaped like CitizenReportFormSubmissionsAggregate,
                -- or no rows when the form does not belong to the ngo. Citizen reports have no observer, so responder ids are empty.
                -- Texts, Notes and Attachments are numbered ("Id") and the question aggregates reference them by id
                -- (see "ReferenceAnswers"). Locations lists the Level1..Level5 paths of the submissions and every question
                -- aggregate holds its breakdown per location id in "LocationAggregates" (see "AggregateAnswersByLocation").
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

                                (SELECT COALESCE(SUM("NumberOfQuestionsAnswered"), 0) FROM SUBMISSIONS)::INT AS questions_answered,

                                (SELECT COALESCE(SUM("NumberOfFlaggedAnswers"), 0) FROM SUBMISSIONS)::INT AS flagged_answers,

                                (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                        'SubmissionId', "SubmissionId",
                                        'ResponderId', '00000000-0000-0000-0000-000000000000'::UUID,
                                        'TimeSubmitted', "TimeSubmitted",
                                        'Answers', "Answers",
                                        'NumberOfQuestionsAnswered', "NumberOfQuestionsAnswered",
                                        'NumberOfFlaggedAnswers', "NumberOfFlaggedAnswers",
                                        'Level1', "Level1",
                                        'Level2', "Level2",
                                        'Level3', "Level3",
                                        'Level4', "Level4",
                                        'Level5', "Level5")), '[]'::JSONB)
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

                    v_aggregates := "AggregateAnswers"(v_form.questions, v_collected.submissions);
                    v_texts := "ExtractTexts"(v_aggregates);
                    v_notes := "NumberItems"(v_collected.notes);
                    v_attachments := "NumberItems"(v_collected.attachments);
                    v_locations := "AggregateAnswersByLocation"(v_form.questions, v_collected.submissions, v_texts -> 'Ids', v_notes, v_attachments);

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
                               "AttachLocationAggregates"("ReferenceAnswers"(v_aggregates, v_texts -> 'Ids', v_notes, v_attachments, TRUE), v_locations -> 'ByQuestion'),
                               v_notes,
                               v_attachments,
                               v_texts -> 'Texts',
                               v_locations -> 'Locations';
                END;
                $$ LANGUAGE plpgsql;
                """);
        }
    }
}
