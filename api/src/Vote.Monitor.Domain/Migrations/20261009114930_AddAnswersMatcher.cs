using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddAnswersMatcher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "AnswerMatches"(jsonb, uuid, text, jsonb, jsonb);""");
            migrationBuilder.Sql("""
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

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "AnswerMatches"(jsonb, uuid, text, jsonb, jsonb);""");

        }
    }
}
