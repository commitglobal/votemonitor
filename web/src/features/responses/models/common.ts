import type {
  BaseQuestion,
  DateQuestion,
  NumberQuestion,
  RatingQuestion,
  SingleSelectQuestion,
  TextQuestion,
} from '@/common/types';

export enum SubmissionType {
  FormSubmission = 'FormSubmission',
  QuickReport = 'QuickReport',
  CitizenReport = 'CitizenReport',
  IncidentReport = 'IncidentReport',
}

type AttachmentsAndNotesData = {
  submissionType: SubmissionType;
  submissionId: string;
  timeSubmitted: string;
  questionId: string;
};

export interface Note extends AttachmentsAndNotesData {
  type: 'Note';
  text: string;
}

export interface Attachment extends AttachmentsAndNotesData {
  type: 'Attachment';
  fileName: string;
  filePath: string;
  mimeType: string;
  presignedUrl: string;
  uploadedFileName: string;
  urlValidityInSeconds: string;
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
