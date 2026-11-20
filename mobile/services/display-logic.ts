/*
  Display logic: whether a question is shown, given the answers so far. Kept free of app modules
  (i18n, Expo) so it can be tested with node --test.
*/
import type {
  MultiSelectAnswer,
  NumberAnswer,
  RatingAnswer,
  SingleSelectAnswer,
  TextAnswer,
  ApiFormAnswer,
} from "./interfaces/answer.type";
import type { ApiFormQuestion, DisplayLogicCondition } from "./interfaces/question.type";

const mapDisplayConditionToMath = (
  answer: string | number,
  target: string,
  condition: DisplayLogicCondition,
) => {
  // numbers and ratings compare as numbers ("10" > "9"); text compares as text
  const numeric = typeof answer === "number";
  const operand1 = answer;
  const operand2 = numeric ? Number(target) : target;
  if (numeric && Number.isNaN(operand2)) return false;
  switch (condition) {
    case "Equals":
      return operand1 === operand2;
    case "GreaterEqual":
      return operand1 >= operand2;
    case "GreaterThan":
      return operand1 > operand2;
    case "LessEqual":
      return operand1 <= operand2;
    case "LessThan":
      return operand1 < operand2;
    case "NotEquals":
      return operand1 !== operand2;
    default:
      return false;
  }
};

// the values date, time and date-time questions send and read back (see date-answers)
export { fromDayIso, fromTimeOfDay, toDayIso, toTimeOfDay } from "../common/utils/date-answers";

/**
 * Whether a select parent's picked options meet a display rule. The rule lists the options in
 * `optionIds`: "AnyOf" needs one of them picked, "All" every one of them.
 * A condition this version doesn't know shows the question, so its answer is never discarded.
 */
const matchesSelectCondition = (
  picked: string[],
  logic: { condition: DisplayLogicCondition; value?: string | null; optionIds?: string[] | null },
) => {
  const targets = logic.optionIds ?? [];
  switch (logic.condition) {
    case "AnyOf":
      return picked.some((id) => targets.includes(id));
    case "All":
      return targets.length > 0 && targets.every((id) => picked.includes(id));
    default:
      // older rules name a single option in value: shown when that option is picked
      return !!logic.value && picked.includes(logic.value);
  }
};

export const shouldDisplayQuestion = (
  q: ApiFormQuestion,
  answers: Record<string, ApiFormAnswer | undefined> | undefined,
) => {
  if (q.displayLogic) {
    if (!answers?.[q.displayLogic?.parentQuestionId]) {
      return false;
    }
    const condition: DisplayLogicCondition = q.displayLogic.condition;
    const parentAnswerType = answers?.[q.displayLogic?.parentQuestionId]?.$answerType;
    const parentAnswer = answers?.[q.displayLogic?.parentQuestionId];
    const value = q.displayLogic?.value ?? "";
    const optionIds = q.displayLogic?.optionIds ?? [];

    let shouldDisplay = false;

    if (!condition || !parentAnswerType || (!value && optionIds.length === 0)) {
      return shouldDisplay;
    }

    switch (parentAnswerType) {
      case "multiSelectAnswer":
        shouldDisplay = matchesSelectCondition(
          (parentAnswer as MultiSelectAnswer).selection.map((option) => option.optionId),
          q.displayLogic,
        );
        break;
      case "singleSelectAnswer":
        shouldDisplay = matchesSelectCondition(
          [(parentAnswer as SingleSelectAnswer).selection.optionId],
          q.displayLogic,
        );
        break;
      case "numberAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as NumberAnswer).value,
          value,
          condition,
        );
        break;
      case "textAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as TextAnswer).text,
          value,
          condition,
        );
        break;
      case "ratingAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as RatingAnswer).value,
          value,
          condition,
        );
        break;
      default:
        shouldDisplay = false;
    }

    return shouldDisplay;
  }

  return true;
};
