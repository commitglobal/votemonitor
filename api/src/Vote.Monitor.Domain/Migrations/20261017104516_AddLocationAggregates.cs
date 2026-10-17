using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationAggregates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                -- Adds a 1-based "Id" to every item of a JSON array.
                CREATE OR REPLACE FUNCTION "NumberItems"(
                    p_items JSONB
                )
                    RETURNS JSONB
                AS
                $$
                SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT('Id', X.idx) || X.item ORDER BY X.idx), '[]'::JSONB)
                FROM JSONB_ARRAY_ELEMENTS(COALESCE(p_items, '[]'::JSONB)) WITH ORDINALITY X(item, idx);
                $$ LANGUAGE sql IMMUTABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Key identifying a text answer or free text of a question: question id, submission id and a hash of the text.
                CREATE OR REPLACE FUNCTION "TextAnswerKey"(
                    p_question_id TEXT,
                    p_text JSONB
                )
                    RETURNS TEXT
                AS
                $$
                SELECT p_question_id || '|' || (p_text ->> 'SubmissionId') || '|' || MD5(COALESCE(p_text ->> 'Value', ''));
                $$ LANGUAGE sql IMMUTABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Numbers the text answers ("Answers") and free texts ("FreeTexts") of answer aggregates.
                -- Returns { "Texts": [{ "Id", "SubmissionId", "ResponderId", "Value" }], "Ids": { <"TextAnswerKey"> : <id> } }.
                CREATE OR REPLACE FUNCTION "ExtractTexts"(
                    p_aggregates JSONB
                )
                    RETURNS JSONB
                AS
                $$
                WITH
                    TEXTS AS (
                        SELECT "TextAnswerKey"(A.key, T.text)                   AS key,
                               T.text,
                               ROW_NUMBER() OVER (ORDER BY A.key, T.kind, T.idx) AS position
                        FROM JSONB_EACH(p_aggregates) A
                                 CROSS JOIN LATERAL (
                            SELECT 1 AS kind, X.text, X.idx
                            FROM JSONB_ARRAY_ELEMENTS(COALESCE(A.value -> 'Answers', '[]'::JSONB)) WITH ORDINALITY X(text, idx)
                            UNION ALL
                            SELECT 2, Y.text, Y.idx
                            FROM JSONB_ARRAY_ELEMENTS(COALESCE(A.value -> 'FreeTexts', '[]'::JSONB)) WITH ORDINALITY Y(text, idx)
                            ) T
                    ),
                    -- one id per distinct text
                    UNIQUE_TEXTS AS (
                        SELECT key, text, ROW_NUMBER() OVER (ORDER BY position) AS id
                        FROM (SELECT DISTINCT ON (key) key, text, position
                              FROM TEXTS
                              ORDER BY key, position) U
                    )
                SELECT JSONB_BUILD_OBJECT(
                               'Texts', COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT('Id', id) || text ORDER BY id), '[]'::JSONB),
                               'Ids', COALESCE(JSONB_OBJECT_AGG(key, id), '{}'::JSONB))
                FROM UNIQUE_TEXTS;
                $$ LANGUAGE sql IMMUTABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Makes answer aggregates reference texts, notes and attachments by id: text answers ("Answers") and free texts
                -- ("FreeTexts") are replaced with "AnswerIds" / "FreeTextIds", and every aggregate gets the ids of its question's notes
                -- ("NoteIds") and attachments ("AttachmentIds"). The question itself is dropped unless p_keep_question.
                -- p_text_ids: "TextAnswerKey" -> text id (see "ExtractTexts").
                -- p_notes, p_attachments: the numbered notes / attachments to reference (see "NumberItems").
                CREATE OR REPLACE FUNCTION "ReferenceAnswers"(
                    p_aggregates JSONB,
                    p_text_ids JSONB,
                    p_notes JSONB,
                    p_attachments JSONB,
                    p_keep_question BOOLEAN
                )
                    RETURNS JSONB
                AS
                $$
                WITH
                    NOTE_IDS AS (
                        SELECT N ->> 'QuestionId' AS question_id, JSONB_AGG(N -> 'Id' ORDER BY (N ->> 'Id')::INT) AS ids
                        FROM JSONB_ARRAY_ELEMENTS(p_notes) N
                        GROUP BY 1
                    ),
                    ATTACHMENT_IDS AS (
                        SELECT F ->> 'QuestionId' AS question_id, JSONB_AGG(F -> 'Id' ORDER BY (F ->> 'Id')::INT) AS ids
                        FROM JSONB_ARRAY_ELEMENTS(p_attachments) F
                        GROUP BY 1
                    )
                SELECT COALESCE(JSONB_OBJECT_AGG(
                                        A.key,
                                        (CASE WHEN p_keep_question THEN A.value ELSE A.value - 'Question' END) - 'Answers' - 'FreeTexts'
                                            || CASE
                                                   WHEN A.value ? 'Answers' THEN JSONB_BUILD_OBJECT(
                                                           'AnswerIds', (SELECT COALESCE(JSONB_AGG(p_text_ids -> "TextAnswerKey"(A.key, T.text) ORDER BY T.idx), '[]'::JSONB)
                                                                         FROM JSONB_ARRAY_ELEMENTS(A.value -> 'Answers') WITH ORDINALITY T(text, idx)))
                                                   WHEN A.value ? 'FreeTexts' THEN JSONB_BUILD_OBJECT(
                                                           'FreeTextIds', (SELECT COALESCE(JSONB_AGG(p_text_ids -> "TextAnswerKey"(A.key, T.text) ORDER BY T.idx), '[]'::JSONB)
                                                                           FROM JSONB_ARRAY_ELEMENTS(A.value -> 'FreeTexts') WITH ORDINALITY T(text, idx)))
                                                   ELSE '{}'::JSONB
                                               END
                                            || JSONB_BUILD_OBJECT(
                                                   'NoteIds', COALESCE((SELECT ids FROM NOTE_IDS WHERE question_id = A.key), '[]'::JSONB),
                                                   'AttachmentIds', COALESCE((SELECT ids FROM ATTACHMENT_IDS WHERE question_id = A.key), '[]'::JSONB))
                                ), '{}'::JSONB)
                FROM JSONB_EACH(p_aggregates) A;
                $$ LANGUAGE sql IMMUTABLE;
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

            migrationBuilder.Sql(
                """
                -- Breaks the answer aggregates of a form down by location: one location per Level1, Level1/Level2, ... Level1..Level5 path
                -- (levels are keyed by their full path because names can repeat under different parents). Submissions without a value
                -- for a level are left out of that level.
                -- Returns {
                --   "Locations": [{ "Id", "Level", "ParentId", "Level1".."Level5", "SubmissionCount", "TotalNumberOfQuestionsAnswered",
                --                   "TotalNumberOfFlaggedAnswers", "PollingStationsCount" }],
                --   "ByQuestion": { <question id>: { <location id>: <answer aggregate, see "ReferenceAnswers"> } }
                -- }
                -- p_submissions: [{ "SubmissionId", "ResponderId", "TimeSubmitted", "Answers", "NumberOfQuestionsAnswered",
                --                   "NumberOfFlaggedAnswers", "Level1".."Level5", "PollingStationId" (not for citizen reports) }]
                -- p_text_ids: "TextAnswerKey" -> text id (see "ExtractTexts").
                -- p_notes, p_attachments: all numbered notes / attachments of p_submissions (see "NumberItems").
                CREATE OR REPLACE FUNCTION "AggregateAnswersByLocation"(
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
                    S AS (
                        SELECT SUB                          AS submission,
                               NULLIF(SUB ->> 'Level1', '') AS l1,
                               NULLIF(SUB ->> 'Level2', '') AS l2,
                               NULLIF(SUB ->> 'Level3', '') AS l3,
                               NULLIF(SUB ->> 'Level4', '') AS l4,
                               NULLIF(SUB ->> 'Level5', '') AS l5
                        FROM JSONB_ARRAY_ELEMENTS(p_submissions) SUB
                    ),
                    GROUPS AS (
                            SELECT 1 AS level, l1, NULL::TEXT AS l2, NULL::TEXT AS l3, NULL::TEXT AS l4, NULL::TEXT AS l5, JSONB_AGG(submission) AS submissions
                            FROM S
                            WHERE l1 IS NOT NULL
                            GROUP BY l1
                            UNION ALL
                            SELECT 2 AS level, l1, l2, NULL::TEXT AS l3, NULL::TEXT AS l4, NULL::TEXT AS l5, JSONB_AGG(submission) AS submissions
                            FROM S
                            WHERE l2 IS NOT NULL
                            GROUP BY l1, l2
                            UNION ALL
                            SELECT 3 AS level, l1, l2, l3, NULL::TEXT AS l4, NULL::TEXT AS l5, JSONB_AGG(submission) AS submissions
                            FROM S
                            WHERE l3 IS NOT NULL
                            GROUP BY l1, l2, l3
                            UNION ALL
                            SELECT 4 AS level, l1, l2, l3, l4, NULL::TEXT AS l5, JSONB_AGG(submission) AS submissions
                            FROM S
                            WHERE l4 IS NOT NULL
                            GROUP BY l1, l2, l3, l4
                            UNION ALL
                            SELECT 5 AS level, l1, l2, l3, l4, l5, JSONB_AGG(submission) AS submissions
                            FROM S
                            WHERE l5 IS NOT NULL
                            GROUP BY l1, l2, l3, l4, l5
                    ),
                    LOCATIONS AS (
                        SELECT ROW_NUMBER() OVER (ORDER BY level, l1, l2, l3, l4, l5) AS id,
                               G.level, G.l1, G.l2, G.l3, G.l4, G.l5,
                               "AggregateLocationGroup"(p_questions, G.submissions, p_text_ids, p_notes, p_attachments) AS stats
                        FROM GROUPS G
                    ),
                    -- the parent of a location is the location one level up with the same path
                    WITH_PARENTS AS (
                        SELECT L.*, P.id AS parent_id
                        FROM LOCATIONS L
                                 LEFT JOIN LOCATIONS P ON P.level = L.level - 1
                            AND P.l1 IS NOT DISTINCT FROM L.l1
                            AND (P.level < 2 OR P.l2 IS NOT DISTINCT FROM L.l2)
                            AND (P.level < 3 OR P.l3 IS NOT DISTINCT FROM L.l3)
                            AND (P.level < 4 OR P.l4 IS NOT DISTINCT FROM L.l4)
                    )
                SELECT JSONB_BUILD_OBJECT(
                               'Locations', (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                                                               'Id', id, 'Level', level, 'ParentId', parent_id,
                                                                               'Level1', l1, 'Level2', l2, 'Level3', l3, 'Level4', l4, 'Level5', l5)
                                                                           || (stats - 'Aggregates') ORDER BY id), '[]'::JSONB)
                                             FROM WITH_PARENTS),
                               'ByQuestion', (SELECT COALESCE(JSONB_OBJECT_AGG(question_id, locations), '{}'::JSONB)
                                              FROM (SELECT A.key AS question_id, JSONB_OBJECT_AGG(L.id, A.value ORDER BY L.id) AS locations
                                                    FROM LOCATIONS L
                                                             CROSS JOIN LATERAL JSONB_EACH(L.stats -> 'Aggregates') A
                                                    GROUP BY A.key) Q)
                       );
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Adds to every answer aggregate its breakdown per location id ("LocationAggregates").
                -- p_by_question: { <question id>: { <location id>: <answer aggregate> } } (see "AggregateAnswersByLocation").
                CREATE OR REPLACE FUNCTION "AttachLocationAggregates"(
                    p_aggregates JSONB,
                    p_by_question JSONB
                )
                    RETURNS JSONB
                AS
                $$
                SELECT COALESCE(JSONB_OBJECT_AGG(
                                        A.key,
                                        A.value || JSONB_BUILD_OBJECT('LocationAggregates', COALESCE(p_by_question -> A.key, '{}'::JSONB))
                                ), '{}'::JSONB)
                FROM JSONB_EACH(p_aggregates) A;
                $$ LANGUAGE sql IMMUTABLE;
                """);

            // the aggregate functions return new columns, so they have to be dropped and recreated
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportsAggregate"(uuid, uuid, uuid, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportsAggregate"(uuid, uuid, text, uuid, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionsAggregate"(uuid, uuid, text, uuid, text, jsonb);""");

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
                -- Aggregates the answers of an incident reporting form. Returns one row shaped like FormSubmissionsAggregate,
                -- or no rows when the form does not belong to the ngo.
                -- Texts, Notes and Attachments are numbered ("Id") and the question aggregates reference them by id
                -- (see "ReferenceAnswers"). Locations lists the Level1..Level5 paths of the submissions and every question
                -- aggregate holds its breakdown per location id in "LocationAggregates" (see "AggregateAnswersByLocation").
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportsAggregate"(uuid, uuid, uuid, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportsAggregate"(uuid, uuid, text, uuid, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionsAggregate"(uuid, uuid, text, uuid, text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "AttachLocationAggregates"(jsonb, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "AggregateAnswersByLocation"(jsonb, jsonb, jsonb, jsonb, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "AggregateLocationGroup"(jsonb, jsonb, jsonb, jsonb, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ReferenceAnswers"(jsonb, jsonb, jsonb, jsonb, boolean);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "ExtractTexts"(jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "TextAnswerKey"(text, jsonb);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "NumberItems"(jsonb);""");

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
    }
}
