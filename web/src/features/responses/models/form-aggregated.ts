import type {
  BaseQuestion,
  DateQuestion,
  FormType,
  NumberQuestion,
  RatingQuestion,
  SingleSelectQuestion,
  TextQuestion,
} from '@/common/types';
import { Attachment, Note } from './common';

export interface Responder {
  responderId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}

export enum QuestionTypeAggregate {
  NumberAnswerAggregate = 'numberAnswerAggregate',
  TextAnswerAggregate = 'textAnswerAggregate',
  DateAnswerAggregate = 'dateAnswerAggregate',
  RatingAnswerAggregate = 'ratingAnswerAggregate',
  SingleSelectAnswerAggregate = 'singleSelectAnswerAggregate',
  MultiSelectAnswerAggregate = 'multiSelectAnswerAggregate',
}

export interface BaseQuestionAggregate {
  questionId: string;
  displayOrder: number;
  answersAggregated: number;
  $questionType: QuestionTypeAggregate;
  question: BaseQuestion;
}

export interface NumberQuestionAggregate extends BaseQuestionAggregate {
  question: NumberQuestion;
  min: number;
  max: number;
  average: number;
}

export interface TextQuestionAggregate extends BaseQuestionAggregate {
  answers: { submissionId: string; responderId: string; value: string }[];
  question: TextQuestion;
}

export interface DateQuestionAggregate extends BaseQuestionAggregate {
  answersHistogram: Record<string, number>;
  question: DateQuestion;
}

export interface RatingQuestionAggregate extends BaseQuestionAggregate {
  answersHistogram: Record<string, number>;
  question: RatingQuestion;
  min: number;
  max: number;
  average: number;
}

export interface SingleSelectQuestionAggregate extends BaseQuestionAggregate {
  answersHistogram: Record<string, number>;
  question: SingleSelectQuestion;
}

export interface MultiSelectQuestionAggregate extends BaseQuestionAggregate {
  answersHistogram: Record<string, number>;
  question: SingleSelectQuestion;
}

export interface FormAggregated {
  submissionsAggregate: {
    electionRoundId: string;
    monitoringNgoId: string;
    formId: string;
    formCode: string;
    formType: FormType;
    name: Record<string, string>;
    description: Record<string, string>;
    defaultLanguage: string;
    languages: string[];
    responders: Responder[];
    pollingStations: Record<string, string[]>;
    submissionCount: number;
    totalNumberOfQuestionsAnswered: number;
    totalNumberOfFlaggedAnswers: number;
    aggregates: Record<
      string,
      | NumberQuestionAggregate
      | TextQuestionAggregate
      | DateQuestionAggregate
      | RatingQuestionAggregate
      | SingleSelectQuestionAggregate
      | MultiSelectQuestionAggregate
    >;
  };
  attachments: Attachment[];
  notes: Note[];
}

export function isDateAggregate(aggregate: BaseQuestionAggregate): aggregate is DateQuestionAggregate {
  return aggregate.$questionType === QuestionTypeAggregate.DateAnswerAggregate;
}

export function isMultiSelectAggregate(aggregate: BaseQuestionAggregate): aggregate is MultiSelectQuestionAggregate {
  return aggregate.$questionType === QuestionTypeAggregate.MultiSelectAnswerAggregate;
}

export function isNumberAggregate(aggregate: BaseQuestionAggregate): aggregate is NumberQuestionAggregate {
  return aggregate.$questionType === QuestionTypeAggregate.NumberAnswerAggregate;
}

export function isRatingAggregate(aggregate: BaseQuestionAggregate): aggregate is RatingQuestionAggregate {
  return aggregate.$questionType === QuestionTypeAggregate.RatingAnswerAggregate;
}

export function isSingleSelectAggregate(aggregate: BaseQuestionAggregate): aggregate is SingleSelectQuestionAggregate {
  return aggregate.$questionType === QuestionTypeAggregate.SingleSelectAnswerAggregate;
}

export function isTextAggregate(aggregate: BaseQuestionAggregate): aggregate is TextQuestionAggregate {
  return aggregate.$questionType === QuestionTypeAggregate.TextAnswerAggregate;
}
