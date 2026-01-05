import z from 'zod'
import { TranslatedString } from './common'
import { Language } from './language'

export enum FormType {
  PSI = 'PSI',
  Opening = 'Opening',
  Voting = 'Voting',
  ClosingAndCounting = 'ClosingAndCounting',
  CitizenReporting = 'CitizenReporting',
  IncidentReporting = 'IncidentReporting',
  Other = 'Other',
}

export const FormTypeList: FormType[] = [
  FormType.PSI,
  FormType.Opening,
  FormType.Voting,
  FormType.ClosingAndCounting,
  FormType.CitizenReporting,
  FormType.IncidentReporting,
  FormType.Other,
]

export enum FormStatus {
  Drafted = 'Drafted',
  Published = 'Published',
  Archived = 'Archived',
}

export const FormStatusList: FormStatus[] = [
  FormStatus.Drafted,
  FormStatus.Published,
  FormStatus.Archived,
]

export enum TranslationStatus {
  Translated = 'Translated',
  MissingTranslations = 'MissingTranslations',
}

export type LanguagesTranslationStatus = Record<Language, TranslationStatus>

export interface FormAccessModel {
  ngoId: string
  name: string
}

export enum DisplayLogicCondition {
  Equals = 'Equals',
  NotEquals = 'NotEquals',
  LessThan = 'LessThan',
  LessEqual = 'LessEqual',
  GreaterThan = 'GreaterThan',
  GreaterEqual = 'GreaterEqual',
  Includes = 'Includes',
}

export enum QuestionType {
  TextQuestionType = 'textQuestion',
  NumberQuestionType = 'numberQuestion',
  DateQuestionType = 'dateQuestion',
  SingleSelectQuestionType = 'singleSelectQuestion',
  MultiSelectQuestionType = 'multiSelectQuestion',
  RatingQuestionType = 'ratingQuestion',
}
export interface DisplayLogic {
  parentQuestionId: string
  condition: DisplayLogicCondition
  value: string
}

export interface BaseQuestionModel {
  id: string
  $questionType: QuestionType
  code: string
  text: TranslatedString
  helptext?: TranslatedString
  displayLogic?: DisplayLogic
}

export interface DateQuestionModel extends BaseQuestionModel {
  $questionType: QuestionType.DateQuestionType
}

export interface TextQuestionModel extends BaseQuestionModel {
  $questionType: QuestionType.TextQuestionType
  inputPlaceholder?: TranslatedString
}
export interface NumberQuestionModel extends BaseQuestionModel {
  $questionType: QuestionType.NumberQuestionType
  inputPlaceholder?: TranslatedString
}

export enum RatingScaleType {
  OneTo3 = 'OneTo3',
  OneTo4 = 'OneTo4',
  OneTo5 = 'OneTo5',
  OneTo6 = 'OneTo6',
  OneTo7 = 'OneTo7',
  OneTo8 = 'OneTo8',
  OneTo9 = 'OneTo9',
  OneTo10 = 'OneTo10',
}

export interface RatingQuestionModel extends BaseQuestionModel {
  upperLabel?: TranslatedString
  lowerLabel?: TranslatedString
  $questionType: QuestionType.RatingQuestionType
  scale: RatingScaleType
}

export interface SelectOption {
  id: string
  text: TranslatedString
  isFlagged: boolean
  isFreeText: boolean
}

export interface SingleSelectQuestionModel extends BaseQuestionModel {
  $questionType: QuestionType.SingleSelectQuestionType
  options: SelectOption[]
}
export interface MultiSelectQuestionModel extends BaseQuestionModel {
  $questionType: QuestionType.MultiSelectQuestionType
  options: SelectOption[]
}
export interface FormModel {
  id: string
  formType: FormType
  code: string
  defaultLanguage: Language
  icon?: string
  name: TranslatedString
  description?: TranslatedString
  status: FormStatus
  languages: Language[]
  lastModifiedOn: string
  lastModifiedBy: string
  numberOfQuestions: number
  languagesTranslationStatus: LanguagesTranslationStatus
  isFormOwner: boolean
  formAccess: FormAccessModel[]
  questions: BaseQuestionModel[]
}

export const formSearchSchema = z.object({
  searchText: z.string().optional(),
  typeFilter: z.enum(FormType).optional(),
  formStatusFilter: z.enum(FormStatus).optional(),
  pageNumber: z.number().default(1),
  pageSize: z.number().default(25),
})

export type FormSearch = z.infer<typeof formSearchSchema>
