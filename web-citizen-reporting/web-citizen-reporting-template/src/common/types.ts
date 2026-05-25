import { z } from "zod";

export interface ListGuidesResponse {
  guides: GuideModel[];
}

export enum GuideType {
  Website = "Website",
  Document = "Document",
  Text = "Text",
}

export interface GuideModel {
  id: string;
  title: string;
  fileName: string;
  mimeType: string;
  guideType: GuideType;
  text: string;
  websiteUrl: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
  filePath: string;
  uploadedFileName: string;
}

export enum QuestionType {
  TextQuestionType = "textQuestion",
  NumberQuestionType = "numberQuestion",
  DateQuestionType = "dateQuestion",
  SingleSelectQuestionType = "singleSelectQuestion",
  MultiSelectQuestionType = "multiSelectQuestion",
  RatingQuestionType = "ratingQuestion",
}

export enum DisplayLogicCondition {
  Equals = "Equals",
  NotEquals = "NotEquals",
  LessThan = "LessThan",
  LessEqual = "LessEqual",
  GreaterThan = "GreaterThan",
  GreaterEqual = "GreaterEqual",
  Includes = "Includes",
}

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
  OneTo3 = "OneTo3",
  OneTo4 = "OneTo4",
  OneTo5 = "OneTo5",
  OneTo6 = "OneTo6",
  OneTo7 = "OneTo7",
  OneTo8 = "OneTo8",
  OneTo9 = "OneTo9",
  OneTo10 = "OneTo10",
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

export type TranslatedString = Record<string, string>;
export type FormModel = {
  id: string;
  code: string;
  name: TranslatedString;
  description: TranslatedString;
  defaultLanguage: string;
  languages: string[];
  numberOfQuestions: number;
  createdOn: string;
  lastModifiedOn: string;
  questions: BaseQuestion[];
  icon?: string;
  displayOrder: number;
};

export type ElectionRoundsAllFormsModel = {
  electionRoundId: string;
  version: string;
  forms: FormModel[];
};

export enum AnswerType {
  TextAnswerType = "textAnswer",
  NumberAnswerType = "numberAnswer",
  DateAnswerType = "dateAnswer",
  SingleSelectAnswerType = "singleSelectAnswer",
  MultiSelectAnswerType = "multiSelectAnswer",
  RatingAnswerType = "ratingAnswer",
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

export type NotificationModel = {
  id: string;
  title: string;
  body: string;
  sender: string;
  sentAt: Date;
};

export type NotificationsModel = {
  ngoName: string;
  notifications: NotificationModel[];
};

export interface LocationNodeModel {
  id: number;
  name: string;
  depth: number;
  parentId?: number; // available for the leafs
  displayOrder?: number;
  locationId?: string;
}

export type ElectionRoundLocationsModel = {
  electionRoundId: string;
  version: string; // cache bust key
  nodes: LocationNodeModel[];
};

export type ElectionsRoundLocationsCacheModel = {
  electionRoundId: string;
  cacheKey: string;
};

export type FormResponseModel = {
  citizenReportId: string;
  formId: string;
  locationId: string;
  answers: BaseAnswer[];
};
