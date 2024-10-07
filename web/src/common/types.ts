import { z } from 'zod';

export type FunctionComponent = React.ReactElement | null;

type HeroIconSVGProps = React.PropsWithoutRef<React.SVGProps<SVGSVGElement>> & React.RefAttributes<SVGSVGElement>;

type IconProps = HeroIconSVGProps & {
  title?: string;
  titleId?: string;
};
export type Heroicon = React.FC<IconProps>;

export type PageParameters = {
  pageNumber: number; // 1-based (the first page is 1)
  pageSize: number;
};

export enum SortOrder {
  asc = 'Asc',
  desc = 'Desc',
}

export type SortParameters = {
  sortColumnName: string;
  sortOrder: SortOrder;
  searchText?: string;
};

export type PageResponse<T> = {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  items: T[];
  isEmpty?: boolean;
};

export type DataTableParameters<TQueryParams = object> = PageParameters &
  SortParameters & { otherParams?: TQueryParams };

export const ZTranslatedString = z.record(z.string());
export type TranslatedString = z.infer<typeof ZTranslatedString>;

export enum QuestionType {
  TextQuestionType = 'textQuestion',
  NumberQuestionType = 'numberQuestion',
  DateQuestionType = 'dateQuestion',
  SingleSelectQuestionType = 'singleSelectQuestion',
  MultiSelectQuestionType = 'multiSelectQuestion',
  RatingQuestionType = 'ratingQuestion',
}

export const ZDisplayLogicCondition = z.enum([
  'Equals',
  'NotEquals',
  'LessThan',
  'LessEqual',
  'GreaterThan',
  'GreaterEqual',
  'Includes',
]);

export type DisplayLogicCondition = z.infer<typeof ZDisplayLogicCondition>;

export interface DisplayLogic {
  parentQuestionId: string;
  condition: DisplayLogicCondition;
  value: string;
}

export interface BaseQuestion {
  id: string;
  $questionType: QuestionType;
  code: string;
  text: TranslatedString;
  helptext?: TranslatedString;
  displayLogic?: DisplayLogic;
}

export interface DateQuestion extends BaseQuestion {
  $questionType: QuestionType.DateQuestionType;
}

export interface TextQuestion extends BaseQuestion {
  $questionType: QuestionType.TextQuestionType;
  inputPlaceholder?: TranslatedString;
}
export interface NumberQuestion extends BaseQuestion {
  $questionType: QuestionType.NumberQuestionType;
  inputPlaceholder?: TranslatedString;
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

export interface RatingQuestion extends BaseQuestion {
  upperLabel?: TranslatedString;
  lowerLabel?: TranslatedString;
  $questionType: QuestionType.RatingQuestionType;
  scale: RatingScaleType;
}

export interface SelectOption {
  id: string;
  text: TranslatedString;
  isFlagged: boolean;
  isFreeText: boolean;
}

export interface SingleSelectQuestion extends BaseQuestion {
  $questionType: QuestionType.SingleSelectQuestionType;
  options: SelectOption[];
}
export interface MultiSelectQuestion extends BaseQuestion {
  $questionType: QuestionType.MultiSelectQuestionType;
  options: SelectOption[];
}

export enum AnswerType {
  TextAnswerType = 'textAnswer',
  NumberAnswerType = 'numberAnswer',
  DateAnswerType = 'dateAnswer',
  SingleSelectAnswerType = 'singleSelectAnswer',
  MultiSelectAnswerType = 'multiSelectAnswer',
  RatingAnswerType = 'ratingAnswer',
}

export const BaseAnswerSchema = z.object({
  $answerType: z.string(),
  questionId: z.string(),
});
export type BaseAnswer = z.infer<typeof BaseAnswerSchema>;

export const TextAnswerSchema = BaseAnswerSchema.extend({
  $answerType: z.literal(AnswerType.TextAnswerType),
  text: z.string().optional(),
});
export type TextAnswer = z.infer<typeof TextAnswerSchema>;

export const NumberAnswerSchema = BaseAnswerSchema.extend({
  $answerType: z.literal(AnswerType.NumberAnswerType),
  value: z.coerce.number().optional(),
});
export type NumberAnswer = z.infer<typeof NumberAnswerSchema>;

export const DateAnswerSchema = BaseAnswerSchema.extend({
  $answerType: z.literal(AnswerType.DateAnswerType),
  date: z.string().datetime({ offset: true }).optional(),
});
export type DateAnswer = z.infer<typeof DateAnswerSchema>;

export const RatingAnswerSchema = BaseAnswerSchema.extend({
  $answerType: z.literal(AnswerType.RatingAnswerType),
  value: z.coerce.number().optional(),
});
export type RatingAnswer = z.infer<typeof RatingAnswerSchema>;

export const SelectedOptionSchema = z.object({
  optionId: z.string().optional(),
  text: z.string().optional().nullable(),
});
export type SelectedOption = z.infer<typeof SelectedOptionSchema>;

export const SingleSelectAnswerSchema = BaseAnswerSchema.extend({
  $answerType: z.literal(AnswerType.SingleSelectAnswerType),
  selection: SelectedOptionSchema.optional(),
});
export type SingleSelectAnswer = z.infer<typeof SingleSelectAnswerSchema>;

export const MultiSelectAnswerSchema = BaseAnswerSchema.extend({
  $answerType: z.literal(AnswerType.MultiSelectAnswerType),
  selection: z.array(SelectedOptionSchema).optional(),
});
export type MultiSelectAnswer = z.infer<typeof MultiSelectAnswerSchema>;

export type ElectionRoundMonitoring = {
  monitoringNgoId: string;
  electionRoundId: string;
  title: string;
  englishTitle: string;
  startDate: string;
  country: string;
  countryId: string;
  isMonitoringNgoForCitizenReporting: boolean;
};

export type LevelNode = {
  id: number;
  name: string;
  depth: number;
  parentId: number;
};

export type UserPayload = {
  'user-role': string;
};

export enum FormSubmissionFollowUpStatus {
  NotApplicable = 'NotApplicable',
  NeedsFollowUp = 'NeedsFollowUp',
  Resolved = 'Resolved',
}

export enum QuickReportFollowUpStatus {
  NotApplicable = 'NotApplicable',
  NeedsFollowUp = 'NeedsFollowUp',
  Resolved = 'Resolved',
}

export enum IncidentReportFollowUpStatus {
  NotApplicable = 'NotApplicable',
  NeedsFollowUp = 'NeedsFollowUp',
  Resolved = 'Resolved',
}

export enum CitizenReportFollowUpStatus {
  NotApplicable = 'NotApplicable',
  NeedsFollowUp = 'NeedsFollowUp',
  Resolved = 'Resolved',
}

export enum QuestionsAnswered {
  None = 'None',
  Some = 'Some',
  All = 'All',
}
export type HistogramData = {
  [bucket: string]: number;
};

export const ZFormType = z.enum(['PSI', 'Opening', 'Voting', 'ClosingAndCounting', 'CitizenReporting', 'IncidentReporting','Other']);

export type FormType = z.infer<typeof ZFormType>;

export const ZTranslationStatus = z.enum(['Translated', 'MissingTranslations']);
export type TranslationStatus = z.infer<typeof ZTranslationStatus>;

const ZLanguagesTranslationStatus = z.record(z.string(), ZTranslationStatus);
export type LanguagesTranslationStatus = z.infer<typeof ZLanguagesTranslationStatus>;


export interface Country {
  id: string;
  iso2: string;
  iso3: string;
  numericCode: string;
  name: string;
  fullName: string;
}

export interface Language {
  id: string;
  code: string;
  name: string;
  nativeName: string;
}