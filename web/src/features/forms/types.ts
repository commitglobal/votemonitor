import { QuestionType, RatingScaleType, ZDisplayLogicCondition } from "@/common/types";
import { z } from "zod";

export const ZTranslatedString = z.record(z.string());

const ZBaseEditQuestionType = z.object({
  questionId: z.string(),
  defaultLanguage: z.string().trim().min(1),
  languageCode: z.string().trim().min(1),
  text: ZTranslatedString,
  helptext: ZTranslatedString,

  hasDisplayLogic: z.boolean().catch(false),
  parentQuestionId: z.string().optional(),
  condition: ZDisplayLogicCondition.optional().catch('Equals'),
  value: z.string().optional(),

  code: z.string().trim().min(1),
});

const ZEditRatingQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.RatingQuestionType),
  lowerLabel: ZTranslatedString,
  upperLabel: ZTranslatedString,
  scale: z.enum([
    RatingScaleType.OneTo3,
    RatingScaleType.OneTo4,
    RatingScaleType.OneTo5,
    RatingScaleType.OneTo6,
    RatingScaleType.OneTo7,
    RatingScaleType.OneTo8,
    RatingScaleType.OneTo9,
    RatingScaleType.OneTo10
  ]),
});

const ZEditDateQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.DateQuestionType)
});

const ZEditNumberQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.NumberQuestionType),
  inputPlaceholder: ZTranslatedString
});

const ZEditTextQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.TextQuestionType),
  inputPlaceholder: ZTranslatedString,
});

const ZEditSelectOptionType = z.object({
  optionId: z.string(),
  text: ZTranslatedString,
  isFlagged: z.boolean(),
  isFreeText: z.boolean()
});

const ZEditMultiSelectQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.MultiSelectQuestionType),
  options: z.array(ZEditSelectOptionType).min(1)
});

const ZEditSingleSelectQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.SingleSelectQuestionType),
  options: z.array(ZEditSelectOptionType).min(1),
});

export const ZEditQuestionType = z.discriminatedUnion('$questionType', [
  ZEditTextQuestionType,
  ZEditNumberQuestionType,
  ZEditDateQuestionType,
  ZEditRatingQuestionType,
  ZEditSingleSelectQuestionType,
  ZEditMultiSelectQuestionType
]);

export type EditSelectOptionType = z.infer<typeof ZEditSelectOptionType>;
export type EditMultiSelectQuestionType = z.infer<typeof ZEditMultiSelectQuestionType>;
export type EditSingleSelectQuestionType = z.infer<typeof ZEditSingleSelectQuestionType>;
export type EditTextQuestionType = z.infer<typeof ZEditTextQuestionType>;
export type EditNumberQuestionType = z.infer<typeof ZEditNumberQuestionType>;
export type EditRatingQuestionType = z.infer<typeof ZEditRatingQuestionType>;
export type EditDateQuestionType = z.infer<typeof ZEditDateQuestionType>;

export type EditQuestionType = z.infer<typeof ZEditQuestionType>;
