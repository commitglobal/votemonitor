import {
  DateAnswerSchema,
  MultiSelectAnswerSchema,
  NumberAnswerSchema,
  QuestionType,
  RatingAnswerSchema,
  SingleSelectAnswerSchema,
  TextAnswerSchema,
  type BaseAnswer,
  type BaseQuestion,
  type DateAnswer,
  type MultiSelectAnswer,
  type MultiSelectQuestion,
  type NumberAnswer,
  type RatingAnswer,
  type RatingQuestion,
  type SingleSelectAnswer,
  type SingleSelectQuestion,
  type TextAnswer,
} from './types';

export function isDateAnswer(answer: BaseAnswer): answer is DateAnswer {
  return DateAnswerSchema.safeParse(answer).success;
}

export function isMultiSelectAnswer(answer: BaseAnswer): answer is MultiSelectAnswer {
  return MultiSelectAnswerSchema.safeParse(answer).success;
}

export function isMultiSelectQuestion(question: BaseQuestion): question is MultiSelectQuestion {
  return question.$questionType === QuestionType.MultiSelectQuestionType;
}

export function isNumberAnswer(answer: BaseAnswer): answer is NumberAnswer {
  return NumberAnswerSchema.safeParse(answer).success;
}

export function isRatingAnswer(answer: BaseAnswer): answer is RatingAnswer {
  return RatingAnswerSchema.safeParse(answer).success;
}

export function isRatingQuestion(question: BaseQuestion): question is RatingQuestion {
  return question.$questionType === QuestionType.RatingQuestionType
}

export function isSingleSelectAnswer(answer: BaseAnswer): answer is SingleSelectAnswer {
  return SingleSelectAnswerSchema.safeParse(answer).success;
}

export function isSingleSelectQuestion(question: BaseQuestion): question is SingleSelectQuestion {
  return question.$questionType === QuestionType.SingleSelectQuestionType;
}

export function isTextAnswer(answer: BaseAnswer): answer is TextAnswer {
  return TextAnswerSchema.safeParse(answer).success;
}
