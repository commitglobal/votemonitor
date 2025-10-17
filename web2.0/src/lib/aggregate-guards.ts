import { QuestionType } from '@/types/form'
import {
  DateQuestionAggregateModel,
  MultiSelectQuestionAggregateModel,
  NumberQuestionAggregateModel,
  RatingQuestionAggregateModel,
  SingleSelectQuestionAggregateModel,
  TextQuestionAggregateModel,
} from '@/types/submissions-aggregate'

export function isDateQuestionAggregate(
  aggregate:
    | NumberQuestionAggregateModel
    | TextQuestionAggregateModel
    | DateQuestionAggregateModel
    | RatingQuestionAggregateModel
    | SingleSelectQuestionAggregateModel
    | MultiSelectQuestionAggregateModel
): aggregate is DateQuestionAggregateModel {
  if (!aggregate) return false
  return aggregate.question.$questionType === QuestionType.DateQuestionType
}

export function isMultiSelectQuestionAggregate(
  aggregate:
    | NumberQuestionAggregateModel
    | TextQuestionAggregateModel
    | DateQuestionAggregateModel
    | RatingQuestionAggregateModel
    | SingleSelectQuestionAggregateModel
    | MultiSelectQuestionAggregateModel
): aggregate is MultiSelectQuestionAggregateModel {
  if (!aggregate) return false
  return (
    aggregate.question.$questionType === QuestionType.MultiSelectQuestionType
  )
}

export function isNumberQuestionAggregate(
  aggregate:
    | NumberQuestionAggregateModel
    | TextQuestionAggregateModel
    | DateQuestionAggregateModel
    | RatingQuestionAggregateModel
    | SingleSelectQuestionAggregateModel
    | MultiSelectQuestionAggregateModel
): aggregate is NumberQuestionAggregateModel {
  if (!aggregate) return false
  return aggregate.question.$questionType === QuestionType.NumberQuestionType
}

export function isRatingQuestionAggregate(
  aggregate:
    | NumberQuestionAggregateModel
    | TextQuestionAggregateModel
    | DateQuestionAggregateModel
    | RatingQuestionAggregateModel
    | SingleSelectQuestionAggregateModel
    | MultiSelectQuestionAggregateModel
): aggregate is RatingQuestionAggregateModel {
  if (!aggregate) return false
  return aggregate.question.$questionType === QuestionType.RatingQuestionType
}

export function isSingleSelectQuestionAggregate(
  aggregate:
    | NumberQuestionAggregateModel
    | TextQuestionAggregateModel
    | DateQuestionAggregateModel
    | RatingQuestionAggregateModel
    | SingleSelectQuestionAggregateModel
    | MultiSelectQuestionAggregateModel
): aggregate is SingleSelectQuestionAggregateModel {
  if (!aggregate) return false
  return (
    aggregate.question.$questionType === QuestionType.SingleSelectQuestionType
  )
}

export function isTextAnswer(
  aggregate:
    | NumberQuestionAggregateModel
    | TextQuestionAggregateModel
    | DateQuestionAggregateModel
    | RatingQuestionAggregateModel
    | SingleSelectQuestionAggregateModel
    | MultiSelectQuestionAggregateModel
): aggregate is TextQuestionAggregateModel {
  if (!aggregate) return false
  return aggregate.question.$questionType === QuestionType.TextQuestionType
}
