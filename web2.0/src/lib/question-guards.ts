import {
  BaseQuestionModel,
  DateQuestionModel,
  MultiSelectQuestionModel,
  NumberQuestionModel,
  QuestionType,
  RatingQuestionModel,
  SingleSelectQuestionModel,
  TextQuestionModel,
} from '@/types/form'

export function isDateQuestion(
  question?: BaseQuestionModel
): question is DateQuestionModel {
  if (!question) return false
  return question.$questionType === QuestionType.DateQuestionType
}

export function isMultiSelectQuestion(
  question?: BaseQuestionModel
): question is MultiSelectQuestionModel {
  if (!question) return false
  return question.$questionType === QuestionType.MultiSelectQuestionType
}

export function isNumberQuestion(
  question?: BaseQuestionModel
): question is NumberQuestionModel {
  if (!question) return false
  return question.$questionType === QuestionType.NumberQuestionType
}

export function isRatingQuestion(
  question?: BaseQuestionModel
): question is RatingQuestionModel {
  if (!question) return false
  return question.$questionType === QuestionType.RatingQuestionType
}

export function isSingleSelectQuestion(
  question?: BaseQuestionModel
): question is SingleSelectQuestionModel {
  if (!question) return false
  return question.$questionType === QuestionType.SingleSelectQuestionType
}

export function isTextQuestion(
  question?: BaseQuestionModel
): question is TextQuestionModel {
  if (!question) return false
  return question.$questionType === QuestionType.TextQuestionType
}
