using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeAndDateTimeAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "AnswerMatches"(
                    p_answers     jsonb,
                    p_question_id uuid,
                    p_operation   text,
                    p_value1      jsonb DEFAULT NULL,
                    p_value2      jsonb DEFAULT NULL
                )
                RETURNS boolean
                LANGUAGE plpgsql
                IMMUTABLE
                AS $function$
                DECLARE
                    v_answer       jsonb;
                    v_type         text;
                    v_actual       text;
                    v_expected     text;
                    v_value        jsonb;
                    v_item         jsonb;
                    v_found        boolean := false;
                    v_actual_num   numeric;
                    v_expected_num numeric;
                    v_actual_date  timestamptz;
                    v_expected_date timestamptz;
                    v_actual_time  time;
                    v_match        boolean;
                BEGIN
                    -- Validate the input array.
                    IF p_answers IS NULL OR jsonb_typeof(p_answers) <> 'array' THEN
                        RETURN false;
                    END IF;
                    p_operation := regexp_replace(p_operation, '^answer-', '');

                    -- Find the answer for the requested question.
                    SELECT a
                    INTO v_answer
                    FROM jsonb_array_elements(p_answers) AS elements(a)
                    WHERE a->>'QuestionId' = p_question_id::text
                    LIMIT 1;

                    IF v_answer IS NULL THEN
                        -- Empty checks on a missing answer.
                        RETURN p_operation = 'isEmpty';
                    END IF;

                    v_type := v_answer->>'$answerType';

                    -- Handle empty checks before type-specific extraction.
                    IF p_operation IN ('isEmpty', 'isNotEmpty') THEN
                        v_found := CASE v_type
                            WHEN 'textAnswer' THEN
                                NULLIF(btrim(v_answer->>'Text'), '') IS NOT NULL
                            WHEN 'numberAnswer' THEN
                                NULLIF(v_answer->>'Value', '') IS NOT NULL
                            WHEN 'ratingAnswer' THEN
                                NULLIF(v_answer->>'Value', '') IS NOT NULL
                            WHEN 'dateAnswer' THEN
                                NULLIF(v_answer->>'Date', '') IS NOT NULL
                            WHEN 'timeAnswer' THEN
                                NULLIF(v_answer->>'Time', '') IS NOT NULL
                            WHEN 'dateTimeAnswer' THEN
                                NULLIF(v_answer->>'DateTime', '') IS NOT NULL
                            WHEN 'singleSelectAnswer' THEN
                                NULLIF(v_answer->'Selection'->>'OptionId', '') IS NOT NULL
                            WHEN 'multiSelectAnswer' THEN
                                jsonb_typeof(v_answer->'Selection') = 'array'
                                AND jsonb_array_length(v_answer->'Selection') > 0
                            ELSE false
                        END;

                        IF p_operation = 'isEmpty' THEN
                            RETURN NOT COALESCE(v_found, false);
                        ELSE
                            RETURN COALESCE(v_found, false);
                        END IF;
                    END IF;

                    -- Extract the answer value according to its discriminator.
                    CASE v_type
                        WHEN 'textAnswer' THEN
                            v_actual := v_answer->>'Text';

                        WHEN 'numberAnswer', 'ratingAnswer' THEN
                            v_actual := v_answer->>'Value';

                        WHEN 'dateAnswer' THEN
                            v_actual := v_answer->>'Date';
                        WHEN 'timeAnswer' THEN
                            v_actual := v_answer->>'Time';

                        WHEN 'dateTimeAnswer' THEN
                            v_actual := v_answer->>'DateTime';

                        WHEN 'singleSelectAnswer' THEN
                            v_actual := v_answer->'Selection'->>'OptionId';

                        WHEN 'multiSelectAnswer' THEN
                            -- Multi-select is handled separately below.
                            NULL;

                        ELSE
                            RETURN false;
                    END CASE;

                    -- Multi-select comparisons.
                    IF v_type = 'multiSelectAnswer' THEN
                        IF jsonb_typeof(v_answer->'Selection') <> 'array'
                           OR v_answer->'Selection' IS NULL THEN
                            RETURN false;
                        END IF;

                        IF p_operation IN ('eq', 'in') THEN
                            IF p_operation = 'eq'
                               AND jsonb_typeof(p_value1) = 'array' THEN
                                -- Every requested option must be selected.
                                RETURN NOT EXISTS (
                                    SELECT 1
                                    FROM jsonb_array_elements(p_value1) AS requested(item)
                                    WHERE NOT EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(v_answer->'Selection') AS selected(item)
                                        WHERE selected.item->>'OptionId' = requested.item #>> '{}'
                                    )
                                );
                            END IF;

                            RETURN EXISTS (
                                SELECT 1
                                FROM jsonb_array_elements(v_answer->'Selection') AS selected(item)
                                WHERE
                                    CASE
                                        WHEN jsonb_typeof(p_value1) = 'array' THEN
                                            EXISTS (
                                                SELECT 1
                                                FROM jsonb_array_elements(p_value1) AS requested(item)
                                                WHERE selected.item->>'OptionId' = requested.item #>> '{}'
                                            )
                                        ELSE
                                            selected.item->>'OptionId' = p_value1 #>> '{}'
                                    END
                            );
                        ELSIF p_operation IN ('ne', 'notIn') THEN
                            RETURN NOT EXISTS (
                                SELECT 1
                                FROM jsonb_array_elements(v_answer->'Selection') AS selected(item)
                                WHERE
                                    CASE
                                        WHEN jsonb_typeof(p_value1) = 'array' THEN
                                            EXISTS (
                                                SELECT 1
                                                FROM jsonb_array_elements(p_value1) AS requested(item)
                                                WHERE selected.item->>'OptionId' = requested.item #>> '{}'
                                            )
                                        ELSE
                                            selected.item->>'OptionId' = p_value1 #>> '{}'
                                    END
                            );
                        ELSE
                            RETURN false;
                        END IF;
                    END IF;

                    -- A missing scalar value cannot match comparison operators.
                    IF v_actual IS NULL THEN
                        RETURN false;
                    END IF;

                    v_expected := p_value1 #>> '{}';

                    -- Type-specific comparisons.
                    CASE v_type
                        WHEN 'textAnswer' THEN
                            CASE p_operation
                                WHEN 'eq' THEN
                                    RETURN v_actual = v_expected;
                                WHEN 'ne' THEN
                                    RETURN v_actual <> v_expected;
                                WHEN 'contains' THEN
                                    RETURN position(lower(v_expected) IN lower(v_actual)) > 0;
                                WHEN 'notContains' THEN
                                    RETURN position(lower(v_expected) IN lower(v_actual)) = 0;
                                WHEN 'in' THEN
                                    RETURN EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual = values_list.item #>> '{}'
                                    );
                                WHEN 'notIn' THEN
                                    RETURN NOT EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual = values_list.item #>> '{}'
                                    );
                                ELSE
                                    RETURN false;
                            END CASE;

                        WHEN 'numberAnswer', 'ratingAnswer' THEN
                            v_actual_num := v_actual::numeric;

                            IF p_operation = 'between' THEN
                                RETURN v_actual_num BETWEEN
                                    (p_value1 #>> '{}')::numeric
                                    AND (p_value2 #>> '{}')::numeric;
                            END IF;

                            CASE p_operation
                                WHEN 'eq' THEN
                                    RETURN v_actual_num = (p_value1 #>> '{}')::numeric;
                                WHEN 'ne' THEN
                                    RETURN v_actual_num <> (p_value1 #>> '{}')::numeric;
                                WHEN 'lt' THEN
                                    RETURN v_actual_num < (p_value1 #>> '{}')::numeric;
                                WHEN 'lte' THEN
                                    RETURN v_actual_num <= (p_value1 #>> '{}')::numeric;
                                WHEN 'gt' THEN
                                    RETURN v_actual_num > (p_value1 #>> '{}')::numeric;
                                WHEN 'gte' THEN
                                    RETURN v_actual_num >= (p_value1 #>> '{}')::numeric;
                                WHEN 'in' THEN
                                    RETURN EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual_num = (values_list.item #>> '{}')::numeric
                                    );
                                WHEN 'notIn' THEN
                                    RETURN NOT EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual_num = (values_list.item #>> '{}')::numeric
                                    );
                                ELSE
                                    RETURN false;
                            END CASE;

                        WHEN 'dateAnswer', 'dateTimeAnswer' THEN
                            v_actual_date := v_actual::timestamptz;

                            IF p_operation = 'between' THEN
                                RETURN v_actual_date BETWEEN
                                    (p_value1 #>> '{}')::timestamptz
                                    AND (p_value2 #>> '{}')::timestamptz;
                            END IF;

                            CASE p_operation
                                WHEN 'eq' THEN
                                    RETURN v_actual_date = (p_value1 #>> '{}')::timestamptz;
                                WHEN 'ne' THEN
                                    RETURN v_actual_date <> (p_value1 #>> '{}')::timestamptz;
                                WHEN 'lt' THEN
                                    RETURN v_actual_date < (p_value1 #>> '{}')::timestamptz;
                                WHEN 'lte' THEN
                                    RETURN v_actual_date <= (p_value1 #>> '{}')::timestamptz;
                                WHEN 'gt' THEN
                                    RETURN v_actual_date > (p_value1 #>> '{}')::timestamptz;
                                WHEN 'gte' THEN
                                    RETURN v_actual_date >= (p_value1 #>> '{}')::timestamptz;
                                ELSE
                                    RETURN false;
                            END CASE;

                        WHEN 'timeAnswer' THEN
                            v_actual_time := v_actual::time;

                            IF p_operation = 'between' THEN
                                RETURN v_actual_time BETWEEN
                                    (p_value1 #>> '{}')::time
                                    AND (p_value2 #>> '{}')::time;
                            END IF;

                            CASE p_operation
                                WHEN 'eq' THEN
                                    RETURN v_actual_time = (p_value1 #>> '{}')::time;
                                WHEN 'ne' THEN
                                    RETURN v_actual_time <> (p_value1 #>> '{}')::time;
                                WHEN 'lt' THEN
                                    RETURN v_actual_time < (p_value1 #>> '{}')::time;
                                WHEN 'lte' THEN
                                    RETURN v_actual_time <= (p_value1 #>> '{}')::time;
                                WHEN 'gt' THEN
                                    RETURN v_actual_time > (p_value1 #>> '{}')::time;
                                WHEN 'gte' THEN
                                    RETURN v_actual_time >= (p_value1 #>> '{}')::time;
                                ELSE
                                    RETURN false;
                            END CASE;
                        WHEN 'singleSelectAnswer' THEN
                            CASE p_operation
                                WHEN 'eq' THEN
                                    RETURN v_actual = v_expected;
                                WHEN 'ne' THEN
                                    RETURN v_actual <> v_expected;
                                WHEN 'in' THEN
                                    RETURN EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual = values_list.item #>> '{}'
                                    );
                                WHEN 'notIn' THEN
                                    RETURN NOT EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual = values_list.item #>> '{}'
                                    );
                                ELSE
                                    RETURN false;
                            END CASE;

                        ELSE
                            RETURN false;
                    END CASE;

                EXCEPTION
                    WHEN invalid_text_representation
                       OR invalid_datetime_format
                       OR datetime_field_overflow
                       OR numeric_value_out_of_range
                       OR division_by_zero
                    THEN
                        RETURN false;
                END;
                $function$;
                """);

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
                                            ), '{}'::JSONB)
                            FROM JSONB_EACH(v_aggregates) E);
                END;
                $$ LANGUAGE plpgsql;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "AnswerMatches"(
                    p_answers     jsonb,
                    p_question_id uuid,
                    p_operation   text,
                    p_value1      jsonb DEFAULT NULL,
                    p_value2      jsonb DEFAULT NULL
                )
                RETURNS boolean
                LANGUAGE plpgsql
                IMMUTABLE
                AS $function$
                DECLARE
                    v_answer       jsonb;
                    v_type         text;
                    v_actual       text;
                    v_expected     text;
                    v_value        jsonb;
                    v_item         jsonb;
                    v_found        boolean := false;
                    v_actual_num   numeric;
                    v_expected_num numeric;
                    v_actual_date  timestamptz;
                    v_expected_date timestamptz;
                    v_match        boolean;
                BEGIN
                    -- Validate the input array.
                    IF p_answers IS NULL OR jsonb_typeof(p_answers) <> 'array' THEN
                        RETURN false;
                    END IF;
                    p_operation := regexp_replace(p_operation, '^answer-', '');

                    -- Find the answer for the requested question.
                    SELECT a
                    INTO v_answer
                    FROM jsonb_array_elements(p_answers) AS elements(a)
                    WHERE a->>'QuestionId' = p_question_id::text
                    LIMIT 1;

                    IF v_answer IS NULL THEN
                        -- Empty checks on a missing answer.
                        RETURN p_operation = 'isEmpty';
                    END IF;

                    v_type := v_answer->>'$answerType';

                    -- Handle empty checks before type-specific extraction.
                    IF p_operation IN ('isEmpty', 'isNotEmpty') THEN
                        v_found := CASE v_type
                            WHEN 'textAnswer' THEN
                                NULLIF(btrim(v_answer->>'Text'), '') IS NOT NULL
                            WHEN 'numberAnswer' THEN
                                NULLIF(v_answer->>'Value', '') IS NOT NULL
                            WHEN 'ratingAnswer' THEN
                                NULLIF(v_answer->>'Value', '') IS NOT NULL
                            WHEN 'dateAnswer' THEN
                                NULLIF(v_answer->>'Date', '') IS NOT NULL
                            WHEN 'singleSelectAnswer' THEN
                                NULLIF(v_answer->'Selection'->>'OptionId', '') IS NOT NULL
                            WHEN 'multiSelectAnswer' THEN
                                jsonb_typeof(v_answer->'Selection') = 'array'
                                AND jsonb_array_length(v_answer->'Selection') > 0
                            ELSE false
                        END;

                        IF p_operation = 'isEmpty' THEN
                            RETURN NOT COALESCE(v_found, false);
                        ELSE
                            RETURN COALESCE(v_found, false);
                        END IF;
                    END IF;

                    -- Extract the answer value according to its discriminator.
                    CASE v_type
                        WHEN 'textAnswer' THEN
                            v_actual := v_answer->>'Text';

                        WHEN 'numberAnswer', 'ratingAnswer' THEN
                            v_actual := v_answer->>'Value';

                        WHEN 'dateAnswer' THEN
                            v_actual := v_answer->>'Date';

                        WHEN 'singleSelectAnswer' THEN
                            v_actual := v_answer->'Selection'->>'OptionId';

                        WHEN 'multiSelectAnswer' THEN
                            -- Multi-select is handled separately below.
                            NULL;

                        ELSE
                            RETURN false;
                    END CASE;

                    -- Multi-select comparisons.
                    IF v_type = 'multiSelectAnswer' THEN
                        IF jsonb_typeof(v_answer->'Selection') <> 'array'
                           OR v_answer->'Selection' IS NULL THEN
                            RETURN false;
                        END IF;

                        IF p_operation IN ('eq', 'in') THEN
                            IF p_operation = 'eq'
                               AND jsonb_typeof(p_value1) = 'array' THEN
                                -- Every requested option must be selected.
                                RETURN NOT EXISTS (
                                    SELECT 1
                                    FROM jsonb_array_elements(p_value1) AS requested(item)
                                    WHERE NOT EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(v_answer->'Selection') AS selected(item)
                                        WHERE selected.item->>'OptionId' = requested.item #>> '{}'
                                    )
                                );
                            END IF;

                            RETURN EXISTS (
                                SELECT 1
                                FROM jsonb_array_elements(v_answer->'Selection') AS selected(item)
                                WHERE
                                    CASE
                                        WHEN jsonb_typeof(p_value1) = 'array' THEN
                                            EXISTS (
                                                SELECT 1
                                                FROM jsonb_array_elements(p_value1) AS requested(item)
                                                WHERE selected.item->>'OptionId' = requested.item #>> '{}'
                                            )
                                        ELSE
                                            selected.item->>'OptionId' = p_value1 #>> '{}'
                                    END
                            );
                        ELSIF p_operation IN ('ne', 'notIn') THEN
                            RETURN NOT EXISTS (
                                SELECT 1
                                FROM jsonb_array_elements(v_answer->'Selection') AS selected(item)
                                WHERE
                                    CASE
                                        WHEN jsonb_typeof(p_value1) = 'array' THEN
                                            EXISTS (
                                                SELECT 1
                                                FROM jsonb_array_elements(p_value1) AS requested(item)
                                                WHERE selected.item->>'OptionId' = requested.item #>> '{}'
                                            )
                                        ELSE
                                            selected.item->>'OptionId' = p_value1 #>> '{}'
                                    END
                            );
                        ELSE
                            RETURN false;
                        END IF;
                    END IF;

                    -- A missing scalar value cannot match comparison operators.
                    IF v_actual IS NULL THEN
                        RETURN false;
                    END IF;

                    v_expected := p_value1 #>> '{}';

                    -- Type-specific comparisons.
                    CASE v_type
                        WHEN 'textAnswer' THEN
                            CASE p_operation
                                WHEN 'eq' THEN
                                    RETURN v_actual = v_expected;
                                WHEN 'ne' THEN
                                    RETURN v_actual <> v_expected;
                                WHEN 'contains' THEN
                                    RETURN position(lower(v_expected) IN lower(v_actual)) > 0;
                                WHEN 'notContains' THEN
                                    RETURN position(lower(v_expected) IN lower(v_actual)) = 0;
                                WHEN 'in' THEN
                                    RETURN EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual = values_list.item #>> '{}'
                                    );
                                WHEN 'notIn' THEN
                                    RETURN NOT EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual = values_list.item #>> '{}'
                                    );
                                ELSE
                                    RETURN false;
                            END CASE;

                        WHEN 'numberAnswer', 'ratingAnswer' THEN
                            v_actual_num := v_actual::numeric;

                            IF p_operation = 'between' THEN
                                RETURN v_actual_num BETWEEN
                                    (p_value1 #>> '{}')::numeric
                                    AND (p_value2 #>> '{}')::numeric;
                            END IF;

                            CASE p_operation
                                WHEN 'eq' THEN
                                    RETURN v_actual_num = (p_value1 #>> '{}')::numeric;
                                WHEN 'ne' THEN
                                    RETURN v_actual_num <> (p_value1 #>> '{}')::numeric;
                                WHEN 'lt' THEN
                                    RETURN v_actual_num < (p_value1 #>> '{}')::numeric;
                                WHEN 'lte' THEN
                                    RETURN v_actual_num <= (p_value1 #>> '{}')::numeric;
                                WHEN 'gt' THEN
                                    RETURN v_actual_num > (p_value1 #>> '{}')::numeric;
                                WHEN 'gte' THEN
                                    RETURN v_actual_num >= (p_value1 #>> '{}')::numeric;
                                WHEN 'in' THEN
                                    RETURN EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual_num = (values_list.item #>> '{}')::numeric
                                    );
                                WHEN 'notIn' THEN
                                    RETURN NOT EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual_num = (values_list.item #>> '{}')::numeric
                                    );
                                ELSE
                                    RETURN false;
                            END CASE;

                        WHEN 'dateAnswer' THEN
                            v_actual_date := v_actual::timestamptz;

                            IF p_operation = 'between' THEN
                                RETURN v_actual_date BETWEEN
                                    (p_value1 #>> '{}')::timestamptz
                                    AND (p_value2 #>> '{}')::timestamptz;
                            END IF;

                            CASE p_operation
                                WHEN 'eq' THEN
                                    RETURN v_actual_date = (p_value1 #>> '{}')::timestamptz;
                                WHEN 'ne' THEN
                                    RETURN v_actual_date <> (p_value1 #>> '{}')::timestamptz;
                                WHEN 'lt' THEN
                                    RETURN v_actual_date < (p_value1 #>> '{}')::timestamptz;
                                WHEN 'lte' THEN
                                    RETURN v_actual_date <= (p_value1 #>> '{}')::timestamptz;
                                WHEN 'gt' THEN
                                    RETURN v_actual_date > (p_value1 #>> '{}')::timestamptz;
                                WHEN 'gte' THEN
                                    RETURN v_actual_date >= (p_value1 #>> '{}')::timestamptz;
                                ELSE
                                    RETURN false;
                            END CASE;

                        WHEN 'singleSelectAnswer' THEN
                            CASE p_operation
                                WHEN 'eq' THEN
                                    RETURN v_actual = v_expected;
                                WHEN 'ne' THEN
                                    RETURN v_actual <> v_expected;
                                WHEN 'in' THEN
                                    RETURN EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual = values_list.item #>> '{}'
                                    );
                                WHEN 'notIn' THEN
                                    RETURN NOT EXISTS (
                                        SELECT 1
                                        FROM jsonb_array_elements(p_value1) AS values_list(item)
                                        WHERE v_actual = values_list.item #>> '{}'
                                    );
                                ELSE
                                    RETURN false;
                            END CASE;

                        ELSE
                            RETURN false;
                    END CASE;

                EXCEPTION
                    WHEN invalid_text_representation
                       OR datetime_field_overflow
                       OR numeric_value_out_of_range
                       OR division_by_zero
                    THEN
                        RETURN false;
                END;
                $function$;
                """);

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
        }
    }
}
