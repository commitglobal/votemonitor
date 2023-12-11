import { z } from "zod";

export enum TFormQuestionType {
  OpenText = "openText",
  MultipleChoiceSingle = "multipleChoiceSingle",
  MultipleChoiceMulti = "multipleChoiceMulti",
  Rating = "rating",
}

export const ZFormChoice = z.object({
  id: z.string(),
  label: z.string(),
});

export type TFormChoice = z.infer<typeof ZFormChoice>;

export const ZFormLogicCondition = z.enum([
  "clicked",
  "submitted",
  "equals",
  "notEquals",
  "lessThan",
  "lessEqual",
  "greaterThan",
  "greaterEqual",
  "includesAll",
  "includesOne",
]);

export type TFormLogicCondition = z.infer<typeof ZFormLogicCondition>;

export const ZFormLogicBase = z.object({
  condition: ZFormLogicCondition.optional(),
  value: z.union([z.string(), z.array(z.string())]).optional(),
  destination: z.union([z.string(), z.literal("end")]).optional(),
});

export const ZFormOpenTextLogic = ZFormLogicBase.extend({
  condition: z.enum(["submitted"]).optional(),
  value: z.undefined(),
});


export const ZFormMultipleChoiceSingleLogic = ZFormLogicBase.extend({
  condition: z.enum(["submitted", "equals", "notEquals"]).optional(),
  value: z.string().optional(),
});

export const ZFormMultipleChoiceMultiLogic = ZFormLogicBase.extend({
  condition: z.enum(["submitted", "includesAll", "includesOne", "equals"]).optional(),
  value: z.union([z.array(z.string()), z.string()]).optional(),
});


const ZFormRatingLogic = ZFormLogicBase.extend({
  condition: z
    .enum([
      "equals",
      "notEquals",
      "lessThan",
      "lessEqual",
      "greaterThan",
      "greaterEqual",
      "submitted",
    ])
    .optional(),
  value: z.union([z.string(), z.number()]).optional(),
});


export const ZFormLogic = z.union([
  ZFormOpenTextLogic,
  ZFormMultipleChoiceSingleLogic,
  ZFormMultipleChoiceMultiLogic,
  ZFormRatingLogic,
]);

export type TFormLogic = z.infer<typeof ZFormLogic>;

const ZFormQuestionBase = z.object({
  id: z.string(),
  $type: z.string(),
  type: z.string(),
  code: z.string(),
  headline: z.string(),
  subheader: z.string().optional(),
  range: z.union([z.literal(5), z.literal(3), z.literal(4), z.literal(7), z.literal(10)]).optional(),
  logic: z.array(ZFormLogic).optional(),
  isDraft: z.boolean().optional(),
});

export const ZFormOpenTextQuestionInputType = z.enum(["text", "number"]);
export type TFormOpenTextQuestionInputType = z.infer<typeof ZFormOpenTextQuestionInputType>;

export const ZFormOpenTextQuestion = ZFormQuestionBase.extend({
  $type: z.literal(TFormQuestionType.OpenText),
  type: z.literal(TFormQuestionType.OpenText),
  placeholder: z.string().optional(),
  longAnswer: z.boolean().optional(),
  logic: z.array(ZFormOpenTextLogic).optional(),
  inputType: ZFormOpenTextQuestionInputType.optional().default("text"),
});

export type TFormOpenTextQuestion = z.infer<typeof ZFormOpenTextQuestion>;



export const ZFormMultipleChoiceSingleQuestion = ZFormQuestionBase.extend({
  $type: z.literal(TFormQuestionType.MultipleChoiceSingle),
  type: z.literal(TFormQuestionType.MultipleChoiceSingle),
  choices: z.array(ZFormChoice),
  logic: z.array(ZFormMultipleChoiceSingleLogic).optional(),
});

export type TFormMultipleChoiceSingleQuestion = z.infer<typeof ZFormMultipleChoiceSingleQuestion>;

export const ZFormMultipleChoiceMultiQuestion = ZFormQuestionBase.extend({
  $type: z.literal(TFormQuestionType.MultipleChoiceMulti),
  type: z.literal(TFormQuestionType.MultipleChoiceMulti),
  choices: z.array(ZFormChoice),
  logic: z.array(ZFormMultipleChoiceMultiLogic).optional(),
});

export type TFormMultipleChoiceMultiQuestion = z.infer<typeof ZFormMultipleChoiceMultiQuestion>;

export const ZFormRatingQuestion = ZFormQuestionBase.extend({
  $type: z.literal(TFormQuestionType.Rating),
  type: z.literal(TFormQuestionType.Rating),
  range: z.union([z.literal(5), z.literal(3), z.literal(4), z.literal(7), z.literal(10)]),
  lowerLabel: z.string(),
  upperLabel: z.string(),
  logic: z.array(ZFormRatingLogic).optional(),
});

export type TFormRatingQuestion = z.infer<typeof ZFormRatingQuestion>;

export const ZFormQuestion = z.union([
  ZFormOpenTextQuestion,
  ZFormMultipleChoiceSingleQuestion,
  ZFormMultipleChoiceMultiQuestion,
  ZFormRatingQuestion,
]);

export type TFormQuestion = z.infer<typeof ZFormQuestion>;

export const ZFormQuestions = z.array(ZFormQuestion);

export type TFormQuestions = z.infer<typeof ZFormQuestions>;

const ZFormStatus = z.enum(["Draft", "InProgress", "Completed", "Archived"]);

export type FormStatus = z.infer<typeof ZFormStatus>;

export const ZForm = z.object({
  id: z.string(),
  createdAt: z.string(),
  updatedAt: z.string().optional(),
  code: z.string(),
  description: z.string(),
  languageCode: z.string(),
  status: ZFormStatus,
  questions: ZFormQuestions,
});

export const ZFormInput = z.object({
  code: z.string().min(1),
  languageCode: z.string().min(1),
  description: z.string().optional(),
  questions: ZFormQuestions.optional(),
});

export type FormModel = z.infer<typeof ZForm>;

export type FormInput = z.infer<typeof ZFormInput>;

export const ZFormTFormQuestionType = z.union([
  z.literal("openText"),
  z.literal("multipleChoiceSingle"),
  z.literal("multipleChoiceMulti"),
  z.literal("rating"),
]);

export type TFormTFormQuestionType = z.infer<typeof ZFormTFormQuestionType>;

export interface TFormQuestionSummary<T> {
  question: T;
  responses: {
    id: string;
    value: string | number | string[];
    updatedAt: Date;
  }[];
}
