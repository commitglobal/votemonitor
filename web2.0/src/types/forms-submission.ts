import z from 'zod'
import { DataSource, SortOrder, type TranslatedString } from './common'
import { FormType } from './form'

export enum FormSubmissionFollowUpStatus {
  NotApplicable = 'NotApplicable',
  NeedsFollowUp = 'NeedsFollowUp',
  Resolved = 'Resolved',
}

export const FormSubmissionFollowUpStatusList: FormSubmissionFollowUpStatus[] =
  [
    FormSubmissionFollowUpStatus.NotApplicable,
    FormSubmissionFollowUpStatus.NeedsFollowUp,
    FormSubmissionFollowUpStatus.Resolved,
  ]

export interface AttachmentModel {
  questionId: string
  fileName: string
  filePath: string
  mimeType: string
  presignedUrl: string
  uploadedFileName: string
  urlValidityInSeconds: string
  timeSubmitted: string
}

export interface NoteModel {
  questionId: string
  submissionId: string
  text: string
  timeSubmitted: string
  monitoringObserverId: string
}

export interface FormSubmissionModel {
  submissionId: string
  email: string
  observerName: string
  ngoName: string
  phoneNumber: string
  formCode: string
  formType: FormType
  formName: TranslatedString
  defaultLanguage: string
  languages: string[]
  monitoringObserverId: string
  numberOfFlaggedAnswers: number
  numberOfQuestionsAnswered: number
  pollingStationId: string
  level1: string
  level2: string
  level3: string
  level4: string
  level5: string
  mediaFilesCount: number
  notesCount: number
  number: string
  isOwnObserver: boolean
  tags: string[]
  timeSubmitted: string
  followUpStatus: FormSubmissionFollowUpStatus
}

export interface FormSubmissionByFormModel {
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

export enum AnswerType {
  TextAnswerType = 'textAnswer',
  NumberAnswerType = 'numberAnswer',
  DateAnswerType = 'dateAnswer',
  SingleSelectAnswerType = 'singleSelectAnswer',
  MultiSelectAnswerType = 'multiSelectAnswer',
  RatingAnswerType = 'ratingAnswer',
}

export interface BaseAnswer {
  $answerType: string
  questionId: string
}

export interface TextAnswer extends BaseAnswer {
  $answerType: AnswerType.TextAnswerType
  text?: string
}

export interface NumberAnswer extends BaseAnswer {
  $answerType: AnswerType.NumberAnswerType
  value?: number
}

export interface DateAnswer extends BaseAnswer {
  $answerType: AnswerType.DateAnswerType
  date?: string // ISO datetime string with offset
}

export interface RatingAnswer extends BaseAnswer {
  $answerType: AnswerType.RatingAnswerType
  value?: number
}

export interface SelectedOption {
  optionId?: string
  text?: string | null
}

export interface SingleSelectAnswer extends BaseAnswer {
  $answerType: AnswerType.SingleSelectAnswerType
  selection?: SelectedOption
}

export interface MultiSelectAnswer extends BaseAnswer {
  $answerType: AnswerType.MultiSelectAnswerType
  selection?: SelectedOption[]
}

export interface FormSubmissionDetailedModel {
  submissionId: string
  email: string
  observerName: string
  ngoName: string
  phoneNumber: string
  formId: string
  monitoringObserverId: string
  numberOfFlaggedAnswers: number
  numberOfQuestionsAnswered: number
  pollingStationId: string
  level1: string
  level2: string
  level3: string
  level4: string
  level5: string
  mediaFilesCount: number
  notesCount: number
  number: string
  isOwnObserver: boolean
  tags: string[]
  timeSubmitted: string
  followUpStatus: FormSubmissionFollowUpStatus
  attachments: AttachmentModel[]
  notes: NoteModel[]
  answers: BaseAnswer[]
}

export enum QuestionsAnswered {
  None = 'None',
  Some = 'Some',
  All = 'All',
}

export const QuestionsAnsweredList: QuestionsAnswered[] = [
  QuestionsAnswered.None,
  QuestionsAnswered.Some,
  QuestionsAnswered.All,
]

export const formSubmissionsSearchSchema = z.object({
  searchText: z.string().optional(),
  formTypeFilter: z.enum(FormType).optional(),
  level1Filter: z.string().optional(),
  level2Filter: z.string().optional(),
  level3Filter: z.string().optional(),
  level4Filter: z.string().optional(),
  level5Filter: z.string().optional(),
  pollingStationNumberFilter: z.string().optional(),
  hasFlaggedAnswers: z.string().optional(),
  monitoringObserverId: z.string().optional(),
  tagsFilter: z.array(z.string()).optional().catch([]).optional(),
  followUpStatus: z.enum(FormSubmissionFollowUpStatus).optional(),

  questionsAnswered: z.enum(QuestionsAnswered).optional(),
  hasNotes: z.string().optional(),
  hasAttachments: z.string().optional(),
  formId: z.string().optional(),

  submissionsFromDate: z.coerce.date().optional(),
  submissionsToDate: z.coerce.date().optional(),
  coalitionMemberId: z.string().optional(),
  dataSource: z.enum(DataSource).optional().default(DataSource.Ngo),
  sortColumnName: z.string().optional(),
  sortOrder: z.enum(SortOrder).optional(),
  pageNumber: z.number().optional(),
  pageSize: z.number().optional(),
})

export type FormSubmissionsSearch = z.infer<typeof formSubmissionsSearchSchema>
