import {
  AnswerType,
  QuestionType,
  RatingScaleType,
  type MultiSelectAnswer,
  type NumberAnswer,
  type RatingAnswer,
  type SingleSelectAnswer,
  type TextAnswer,
} from "@/common/types";

export const mapFormDataToAnswer = (
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
      let selectionArray = value
        ? value.map((val: string) => {
            return { optionId: val };
          })
        : [];
      const multiselectAnswer: MultiSelectAnswer = {
        $answerType: AnswerType.MultiSelectAnswerType,
        questionId,
        selection: selectionArray,
      };
      return multiselectAnswer;

    case QuestionType.SingleSelectQuestionType:
      const singleSelectAnswer: SingleSelectAnswer = {
        $answerType: AnswerType.SingleSelectAnswerType,
        questionId,
        selection: { optionId: value },
      };

      return singleSelectAnswer;

    case QuestionType.RatingQuestionType:
      const ratingAnswer: RatingAnswer = {
        $answerType: AnswerType.RatingAnswerType,
        questionId,
        value: Number(value),
      };

      return ratingAnswer;

    default:
      return value;
  }
};

export function ratingScaleToNumber(scale: RatingScaleType): number {
  switch (scale) {
    case RatingScaleType.OneTo3: {
      return 3;
    }
    case RatingScaleType.OneTo4: {
      return 4;
    }
    case RatingScaleType.OneTo5: {
      return 5;
    }
    case RatingScaleType.OneTo6: {
      return 6;
    }
    case RatingScaleType.OneTo7: {
      return 7;
    }
    case RatingScaleType.OneTo8: {
      return 8;
    }
    case RatingScaleType.OneTo9: {
      return 9;
    }
    case RatingScaleType.OneTo10: {
      return 10;
    }
    default: {
      return 5;
    }
  }
}
