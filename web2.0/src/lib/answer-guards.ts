import {
  AnswerType,
  BaseAnswer,
  DateAnswer,
  MultiSelectAnswer,
  NumberAnswer,
  RatingAnswer,
  SingleSelectAnswer,
  TextAnswer,
} from '@/types/forms-submission'

export function isDateAnswer(answer: BaseAnswer): answer is DateAnswer {
  if (!answer) return false
  return answer.$answerType === AnswerType.DateAnswerType
}

export function isMultiSelectAnswer(
  answer: BaseAnswer
): answer is MultiSelectAnswer {
  if (!answer) return false
  return answer.$answerType === AnswerType.MultiSelectAnswerType
}

export function isNumberAnswer(answer: BaseAnswer): answer is NumberAnswer {
  if (!answer) return false
  return answer.$answerType === AnswerType.NumberAnswerType
}

export function isRatingAnswer(answer: BaseAnswer): answer is RatingAnswer {
  if (!answer) return false
  return answer.$answerType === AnswerType.RatingAnswerType
}

export function isSingleSelectAnswer(
  answer: BaseAnswer
): answer is SingleSelectAnswer {
  if (!answer) return false
  return answer.$answerType === AnswerType.SingleSelectAnswerType
}

export function isTextAnswer(answer: BaseAnswer): answer is TextAnswer {
  if (!answer) return false
  return answer.$answerType === AnswerType.TextAnswerType
}
