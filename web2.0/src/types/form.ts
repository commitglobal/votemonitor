import z from 'zod'
import { TranslatedString } from './common'

export enum FormType {
  PSI = 'PSI',
  Opening = 'Opening',
  Voting = 'Voting',
  ClosingAndCounting = 'ClosingAndCounting',
  CitizenReporting = 'CitizenReporting',
  IncidentReporting = 'IncidentReporting',
  Other = 'Other',
}

export enum FormStatus {
  Drafted = 'Drafted',
  Published = 'Published',
  Obsolete = 'Obsolete',
}

export enum LanguagesTranslationStatus {
  Translated = 'Translated',
  MissingTranslations = 'MissingTranslations',
}

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

export interface DisplayLogic {
  parentQuestionId: string
  condition: DisplayLogicCondition
  value: string
}

export interface BaseQuestion {
  id: string
  $questionType: QuestionType
  code: string
  text: TranslatedString
  helptext?: TranslatedString
  displayLogic?: DisplayLogic
}

export interface DateQuestion extends BaseQuestion {
  $questionType: QuestionType.DateQuestionType
}

export interface TextQuestion extends BaseQuestion {
  $questionType: QuestionType.TextQuestionType
  inputPlaceholder?: TranslatedString
}
export interface NumberQuestion extends BaseQuestion {
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

export enum QuestionType {
  TextQuestionType = 'textQuestion',
  NumberQuestionType = 'numberQuestion',
  DateQuestionType = 'dateQuestion',
  SingleSelectQuestionType = 'singleSelectQuestion',
  MultiSelectQuestionType = 'multiSelectQuestion',
  RatingQuestionType = 'ratingQuestion',
}
export interface RatingQuestion extends BaseQuestion {
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

export interface SingleSelectQuestion extends BaseQuestion {
  $questionType: QuestionType.SingleSelectQuestionType
  options: SelectOption[]
}
export interface MultiSelectQuestion extends BaseQuestion {
  $questionType: QuestionType.MultiSelectQuestionType
  options: SelectOption[]
}
export interface FormModel {
  id: string
  formType: FormType
  code: string
  defaultLanguage: string
  icon?: string
  name: TranslatedString
  description?: TranslatedString
  status: FormStatus
  languages: string[]
  lastModifiedOn: string
  lastModifiedBy: string
  numberOfQuestions: number
  languagesTranslationStatus: LanguagesTranslationStatus
  isFormOwner: boolean
  formAccess: FormAccessModel[]
  questions: BaseQuestion[]
}

export const formSearchSchema = z.object({
  searchText: z.string().optional(),
  formTypeFilter: z.enum(FormType).optional(),
  formStatus: z.enum(FormStatus).optional(),
})

export type FormSearch = z.infer<typeof formSearchSchema>
