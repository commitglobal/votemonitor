import { type TranslatedString } from './common'
import {
  BaseQuestionModel,
  DateQuestionModel,
  FormType,
  NumberQuestionModel,
  RatingQuestionModel,
  SingleSelectQuestionModel,
  TextQuestionModel,
  MultiSelectQuestionModel,
} from './form'

export enum QuestionTypeAggregate {
  NumberAnswerAggregate = 'numberAnswerAggregate',
  TextAnswerAggregate = 'textAnswerAggregate',
  DateAnswerAggregate = 'dateAnswerAggregate',
  RatingAnswerAggregate = 'ratingAnswerAggregate',
  SingleSelectAnswerAggregate = 'singleSelectAnswerAggregate',
  MultiSelectAnswerAggregate = 'multiSelectAnswerAggregate',
}

export interface BaseQuestionAggregateModel {
  questionId: string
  displayOrder: number
  answersAggregated: number
  $questionType: QuestionTypeAggregate
  question: BaseQuestionModel
}
export interface NumberQuestionAggregateModel
  extends BaseQuestionAggregateModel {
  question: NumberQuestionModel
  min: number
  max: number
  average: number
}

export interface TextQuestionAggregateModel extends BaseQuestionAggregateModel {
  answers: { submissionId: string; responderId: string; value: string }[]
  question: TextQuestionModel
}

export interface DateQuestionAggregateModel extends BaseQuestionAggregateModel {
  answersHistogram: Record<string, number>
  question: DateQuestionModel
}

export interface RatingQuestionAggregateModel
  extends BaseQuestionAggregateModel {
  answersHistogram: Record<string, number>
  question: RatingQuestionModel
  min: number
  max: number
  average: number
}

export interface SingleSelectQuestionAggregateModel
  extends BaseQuestionAggregateModel {
  answersHistogram: Record<string, number>
  question: SingleSelectQuestionModel
}

export interface MultiSelectQuestionAggregateModel
  extends BaseQuestionAggregateModel {
  answersHistogram: Record<string, number>
  question: MultiSelectQuestionModel
}

export interface ResponderModel {
  responderId: string
  displayName: string
  email: string
  phoneNumber: string
}

export interface AggregatedAttachmentModel {
  questionId: string
  fileName: string
  filePath: string
  mimeType: string
  presignedUrl: string
  uploadedFileName: string
  urlValidityInSeconds: string
  timeSubmitted: string
  submissionId: string
}

export interface AggregatedNoteModel {
  questionId: string
  text: string
  timeSubmitted: string
  submissionId: string
}

export interface AggregatedFormSubmissionsModel {
  submissionsAggregate: {
    electionRoundId: string
    monitoringNgoId: string
    formId: string
    formCode: string
    formType: FormType
    name: Record<string, string>
    description: Record<string, string>
    defaultLanguage: string
    languages: string[]
    responders: ResponderModel[]
    submissionCount: number
    totalNumberOfQuestionsAnswered: number
    totalNumberOfFlaggedAnswers: number
    aggregates: Record<
      string,
      | NumberQuestionAggregateModel
      | TextQuestionAggregateModel
      | DateQuestionAggregateModel
      | RatingQuestionAggregateModel
      | SingleSelectQuestionAggregateModel
      | MultiSelectQuestionAggregateModel
    >
  }
  attachments: AggregatedAttachmentModel[]
  notes: AggregatedNoteModel[]
}

export interface AggregatedFormSubmissionsTableRow {
  formId: string
  formCode: string
  formType: FormType
  formName: TranslatedString
  defaultLanguage: string
  numberOfSubmissions: number
  numberOfFlaggedAnswers: number
  numberOfNotes: number
  numberOfMediaFiles: number
}
