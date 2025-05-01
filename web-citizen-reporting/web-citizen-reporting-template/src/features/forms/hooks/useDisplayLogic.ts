import {
  DisplayLogicCondition,
  QuestionType,
  type BaseQuestion,
} from "@/common/types";
import {
  isMultiSelectAnswer,
  isNumberAnswer,
  isRatingAnswer,
  isSingleSelectAnswer,
  isTextAnswer,
} from "@/lib/utils";
import { useEffect } from "react";
import { useFormMaps } from "./useFormMaps";

const mapDisplayConditionToMath = (
  operand1: string,
  operand2: string,
  condition: DisplayLogicCondition
) => {
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

export const useFormDisplayLogic = (questions: BaseQuestion[]) => {
  const { answersMap, questionsMap } = useFormMaps(questions);

  const shouldDisplayQuestion = (question: BaseQuestion) => {
    if (!question.displayLogic) return true;
    const parentQuestionId = question.displayLogic.parentQuestionId;
    const parentAnswer = answersMap.get(parentQuestionId) ?? null;
    if (!parentAnswer) return false;

    const parentQuestionType =
      questionsMap.get(parentQuestionId)?.$questionType;
    const condition = question.displayLogic.condition;
    const value = question.displayLogic.value;
    let shouldDisplay = false;

    switch (parentQuestionType) {
      case QuestionType.NumberQuestionType:
        if (
          !parentAnswer ||
          !parentAnswer.value ||
          !isNumberAnswer(parentAnswer)
        )
          break;
        shouldDisplay = mapDisplayConditionToMath(
          parentAnswer.value.toString(),
          value,
          condition
        );
        break;

      case QuestionType.TextQuestionType:
        if (!parentAnswer || !parentAnswer.text || !isTextAnswer(parentAnswer))
          break;

        shouldDisplay = mapDisplayConditionToMath(
          parentAnswer.text,
          value,
          condition
        );

        break;

      case QuestionType.MultiSelectQuestionType:
        if (
          !parentAnswer ||
          !parentAnswer.selection ||
          !isMultiSelectAnswer(parentAnswer)
        )
          break;
        shouldDisplay = !!parentAnswer.selection.find(
          (option) => option.optionId === value
        );
        break;

      case QuestionType.SingleSelectQuestionType:
        if (
          !parentAnswer ||
          !parentAnswer.selection ||
          !isSingleSelectAnswer(parentAnswer)
        )
          break;

        shouldDisplay = parentAnswer.selection.optionId === value;
        break;

      case QuestionType.RatingQuestionType:
        if (
          !parentAnswer ||
          !parentAnswer.value ||
          !isRatingAnswer(parentAnswer)
        )
          break;

        shouldDisplay = mapDisplayConditionToMath(
          parentAnswer.value.toString(),
          value,
          condition
        );
        break;

      default:
        break;
    }
    return shouldDisplay;
  };

  useEffect(() => {
    console.table(answersMap);
  }, [answersMap]); // use JSON.stringify to track deep object changes

  return { shouldDisplayQuestion };
};
