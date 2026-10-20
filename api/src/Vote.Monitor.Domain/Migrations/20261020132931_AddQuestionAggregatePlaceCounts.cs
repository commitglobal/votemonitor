using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionAggregatePlaceCounts : Migration
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
                -- Every aggregate also counts the distinct polling stations ("PollingStationsCount", only when the submissions carry
                -- "pollingStationId") and citizen report locations ("LocationsCount", only when they carry "locationId") of the
                -- submissions that answered the question.
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
                                                     WHEN v_has_polling_stations THEN JSONB_BUILD_OBJECT('PollingStationsCount', 0)
                                                     ELSE '{}'::JSONB
                                            END || CASE
                                                     WHEN v_has_locations THEN JSONB_BUILD_OBJECT('LocationsCount', 0)
                                                     ELSE '{}'::JSONB
                                            END || CASE
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
                        -- per question: number of answers, distinct polling stations / citizen report locations they came from,
                        -- and min/max/average for number and rating answers
                        (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, stats), '{}'::JSONB)
                         FROM (SELECT question_id,
                                      JSONB_BUILD_OBJECT('AnswersAggregated', COUNT(*)) ||
                                      CASE
                                          WHEN v_has_polling_stations THEN JSONB_BUILD_OBJECT('PollingStationsCount', COUNT(DISTINCT polling_station_id))
                                          ELSE '{}'::JSONB
                                      END ||
                                      CASE
                                          WHEN v_has_locations THEN JSONB_BUILD_OBJECT('LocationsCount', COUNT(DISTINCT location_id))
                                          ELSE '{}'::JSONB
                                      END ||
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
