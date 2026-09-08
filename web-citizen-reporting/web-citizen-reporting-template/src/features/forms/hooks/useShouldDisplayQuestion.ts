import {
  AnswerType,
  DisplayLogicCondition,
  type BaseAnswer,
  type BaseQuestion,
  type MultiSelectAnswer,
  type NumberAnswer,
  type RatingAnswer,
  type SingleSelectAnswer,
  type TextAnswer,
} from "@/common/types";
import { useEffect, useState } from "react";
import { useWatch, type Control } from "react-hook-form";

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

export const useShouldDisplayQuestion = ({
  question,
  control,
}: {
  question: BaseQuestion;
  control: Control;
}) => {
  const [shouldDisplayQuestion, setShouldDisplay] = useState(false);

  const parentAnswer: BaseAnswer = useWatch({
    control,
    name: `question-${question.displayLogic?.parentQuestionId}`,
  });

  const updateShouldDisplay = () => {
    let shouldDisplay = false;
    const condition = question.displayLogic!.condition;
    const value = question.displayLogic!.value;

    switch (parentAnswer.$answerType) {
      case AnswerType.NumberAnswerType: {
        const parent = parentAnswer as NumberAnswer;

        if (!parent.value) {
          shouldDisplay = false;
          break;
        }

        shouldDisplay = mapDisplayConditionToMath(
          parent.value!.toString(),
          value,
          condition
        );
        break;
      }

      case AnswerType.TextAnswerType: {
        const answer = parentAnswer as TextAnswer;
        if (!answer.text) break;

        shouldDisplay = mapDisplayConditionToMath(
          answer.text,
          value,
          condition
        );

        break;
      }

      case AnswerType.MultiSelectAnswerType: {
        const parent = parentAnswer as MultiSelectAnswer;

        if (!parent.selection) break;
        shouldDisplay = !!parent.selection.find(
          (option) => option.optionId === value
        );
        break;
      }

      case AnswerType.SingleSelectAnswerType: {
        const parent = parentAnswer as SingleSelectAnswer;
        if (!parent.selection) break;

        shouldDisplay = parent.selection.optionId === value;
        break;
      }

      case AnswerType.RatingAnswerType: {
        const parent = parentAnswer as RatingAnswer;
        if (!parent.value) break;

        shouldDisplay = mapDisplayConditionToMath(
          parent.value.toString(),
          value,
          condition
        );
        break;
      }
      default:
        break;
    }
    setShouldDisplay(shouldDisplay);
  };

  useEffect(() => {
    if (!parentAnswer) return;
    updateShouldDisplay();
  }, [parentAnswer]);

  return shouldDisplayQuestion;
};
