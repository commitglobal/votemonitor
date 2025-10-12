import {
  BaseQuestion,
  DateQuestion,
  MultiSelectQuestion,
  NumberQuestion,
  QuestionType,
  RatingQuestion,
  SingleSelectQuestion,
  TextQuestion,
} from '@/types/form'

export function isDateQuestion(
  question?: BaseQuestion
): question is DateQuestion {
  if (!question) return false
  return question.$questionType === QuestionType.DateQuestionType
}

export function isMultiSelectQuestion(
  question?: BaseQuestion
): question is MultiSelectQuestion {
  if (!question) return false
  return question.$questionType === QuestionType.MultiSelectQuestionType
}

export function isNumberQuestion(
  question?: BaseQuestion
): question is NumberQuestion {
  if (!question) return false
  return question.$questionType === QuestionType.NumberQuestionType
}

export function isRatingQuestion(
  question?: BaseQuestion
): question is RatingQuestion {
  if (!question) return false
  return question.$questionType === QuestionType.RatingQuestionType
}

export function isSingleSelectQuestion(
  question?: BaseQuestion
): question is SingleSelectQuestion {
  if (!question) return false
  return question.$questionType === QuestionType.SingleSelectQuestionType
}

export function isTextQuestion(
  question?: BaseQuestion
): question is TextQuestion {
  if (!question) return false
  return question.$questionType === QuestionType.TextQuestionType
}
