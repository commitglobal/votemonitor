import {
  AnswerType,
  QuestionType,
  type BaseQuestion,
  type MultiSelectAnswer,
  type NumberAnswer,
  type RatingAnswer,
  type SingleSelectAnswer,
  type TextAnswer,
} from "@/common/types";
import { useMemo } from "react";
import { useFormContext } from "react-hook-form";

const mapFormDataToAnswer = (
  questionType: QuestionType,
  questionId: string,
  value: any
) => {
  switch (questionType) {
    case QuestionType.NumberQuestionType:
      const numberAnswer: NumberAnswer = {
        $answerType: AnswerType.NumberAnswerType,
        questionId,
        value,
      };
      return numberAnswer;

    case QuestionType.TextQuestionType:
      const textAnswer: TextAnswer = {
        $answerType: AnswerType.TextAnswerType,
        questionId,
        text: value,
      };
      return textAnswer;

    case QuestionType.MultiSelectQuestionType:
      const multiselectAnswer: MultiSelectAnswer = {
        $answerType: AnswerType.MultiSelectAnswerType,
        questionId,
        selection: value.selection.map((val: string) => {
          return { optionId: val };
        }),
      };
      return multiselectAnswer;

    case QuestionType.SingleSelectQuestionType:
      const selectedOptionId = value.selection;
      const singleSelectAnswer: SingleSelectAnswer = {
        $answerType: AnswerType.SingleSelectAnswerType,
        questionId,
        selection: selectedOptionId
          ? { optionId: selectedOptionId }
          : undefined,
      };

      return singleSelectAnswer;

    case QuestionType.RatingQuestionType:
      const ratingAnswer: RatingAnswer = {
        $answerType: AnswerType.RatingAnswerType,
        questionId,
        value,
      };

      return ratingAnswer;

    default:
      return value;
      break;
  }
};

export const useFormMaps = (questions: BaseQuestion[]) => {
  const { watch } = useFormContext();
  const formValues = watch();

  const questionsMap = useMemo(() => {
    return new Map(questions.map((question) => [question.id, question]));
  }, [questions]);

  const answersMap = useMemo(() => {
    return new Map(
      Object.entries(formValues)
        .filter(([key, value]) => key.startsWith("question"))
        .map(([key, value]) => {
          const questionId = key.replace(/^question-/, "");
          const question = questionsMap.get(questionId)!;
          const mappedAnswer = mapFormDataToAnswer(
            question?.$questionType,
            questionId,
            value
          );
          return [questionId, mappedAnswer];
        })
    );
  }, [JSON.stringify(formValues), questionsMap]);

  return { questionsMap, answersMap };
};
