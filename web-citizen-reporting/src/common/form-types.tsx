import {
  DateQuestion,
  MultiSelectQuestion,
  NumberQuestion,
  QuestionType,
  RatingQuestion,
  RatingScaleType,
  SingleSelectQuestion,
  TextQuestion,
  ZDisplayLogicCondition,
  ZFormType,
} from "@/common/types";
import { isNilOrWhitespace, isNotNilOrWhitespace } from "@/lib/utils";
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
  condition: ZDisplayLogicCondition.optional().catch("Equals"),
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
    RatingScaleType.OneTo10,
  ]),
});

const ZEditDateQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.DateQuestionType),
});

const ZEditNumberQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.NumberQuestionType),
  inputPlaceholder: ZTranslatedString,
});

const ZEditTextQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.TextQuestionType),
  inputPlaceholder: ZTranslatedString,
});

const ZEditSelectOptionType = z.object({
  optionId: z.string(),
  text: ZTranslatedString,
  isFlagged: z.boolean(),
  isFreeText: z.boolean(),
});

const ZEditMultiSelectQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.MultiSelectQuestionType),
  options: z.array(ZEditSelectOptionType).min(1),
});

const ZEditSingleSelectQuestionType = ZBaseEditQuestionType.extend({
  $questionType: z.literal(QuestionType.SingleSelectQuestionType),
  options: z.array(ZEditSelectOptionType).min(1),
});

export const ZEditQuestionType = z.discriminatedUnion("$questionType", [
  ZEditTextQuestionType,
  ZEditNumberQuestionType,
  ZEditDateQuestionType,
  ZEditRatingQuestionType,
  ZEditSingleSelectQuestionType,
  ZEditMultiSelectQuestionType,
]);

export type EditSelectOptionType = z.infer<typeof ZEditSelectOptionType>;
export type EditMultiSelectQuestionType = z.infer<
  typeof ZEditMultiSelectQuestionType
>;
export type EditSingleSelectQuestionType = z.infer<
  typeof ZEditSingleSelectQuestionType
>;
export type EditTextQuestionType = z.infer<typeof ZEditTextQuestionType>;
export type EditNumberQuestionType = z.infer<typeof ZEditNumberQuestionType>;
export type EditRatingQuestionType = z.infer<typeof ZEditRatingQuestionType>;
export type EditDateQuestionType = z.infer<typeof ZEditDateQuestionType>;

export type EditQuestionType = z.infer<typeof ZEditQuestionType>;

export const mapToQuestionRequest = (
  q: EditQuestionType
):
  | NumberQuestion
  | TextQuestion
  | RatingQuestion
  | DateQuestion
  | SingleSelectQuestion
  | MultiSelectQuestion => {
  if (q.$questionType === QuestionType.NumberQuestionType) {
    const numberQuestion: NumberQuestion = {
      $questionType: QuestionType.NumberQuestionType,
      code: q.code,
      id: q.questionId,
      text: q.text,
      helptext: q.helptext,
      inputPlaceholder: q.inputPlaceholder,
      displayLogic: q.hasDisplayLogic
        ? {
            condition: q.condition!,
            parentQuestionId: q.parentQuestionId!,
            value: q.value!,
          }
        : undefined,
    };

    return numberQuestion;
  }

  if (q.$questionType === QuestionType.TextQuestionType) {
    const textQuestion: TextQuestion = {
      $questionType: QuestionType.TextQuestionType,
      code: q.code,
      id: q.questionId,
      text: q.text,
      helptext: q.helptext,
      inputPlaceholder: q.inputPlaceholder,
      displayLogic: q.hasDisplayLogic
        ? {
            condition: q.condition!,
            parentQuestionId: q.parentQuestionId!,
            value: q.value!,
          }
        : undefined,
    };

    return textQuestion;
  }

  if (q.$questionType === QuestionType.RatingQuestionType) {
    const ratingQuestion: RatingQuestion = {
      $questionType: QuestionType.RatingQuestionType,
      code: q.code,
      id: q.questionId,
      text: q.text,
      helptext: q.helptext,
      scale: q.scale,
      lowerLabel: q.lowerLabel,
      upperLabel: q.upperLabel,
      displayLogic: q.hasDisplayLogic
        ? {
            condition: q.condition!,
            parentQuestionId: q.parentQuestionId!,
            value: q.value!,
          }
        : undefined,
    };

    return ratingQuestion;
  }

  if (q.$questionType === QuestionType.DateQuestionType) {
    const dateQuestion: DateQuestion = {
      $questionType: QuestionType.DateQuestionType,
      code: q.code,
      id: q.questionId,
      text: q.text,
      helptext: q.helptext,
      displayLogic: q.hasDisplayLogic
        ? {
            condition: q.condition!,
            parentQuestionId: q.parentQuestionId!,
            value: q.value!,
          }
        : undefined,
    };

    return dateQuestion;
  }

  if (q.$questionType === QuestionType.SingleSelectQuestionType) {
    const singleSelectQuestion: SingleSelectQuestion = {
      $questionType: QuestionType.SingleSelectQuestionType,
      code: q.code,
      id: q.questionId,
      text: q.text,
      helptext: q.helptext,
      options: q.options.map((o) => ({
        id: o.optionId,
        isFlagged: o.isFlagged,
        isFreeText: o.isFreeText,
        text: o.text,
      })),
      displayLogic: q.hasDisplayLogic
        ? {
            condition: q.condition!,
            parentQuestionId: q.parentQuestionId!,
            value: q.value!,
          }
        : undefined,
    };

    return singleSelectQuestion;
  }

  if (q.$questionType === QuestionType.MultiSelectQuestionType) {
    const multiSelectQuestion: MultiSelectQuestion = {
      $questionType: QuestionType.MultiSelectQuestionType,
      code: q.code,
      id: q.questionId,
      text: q.text,
      helptext: q.helptext,
      options: q.options.map((o) => ({
        id: o.optionId,
        isFlagged: o.isFlagged,
        isFreeText: o.isFreeText,
        text: o.text,
      })),
      displayLogic: q.hasDisplayLogic
        ? {
            condition: q.condition!,
            parentQuestionId: q.parentQuestionId!,
            value: q.value!,
          }
        : undefined,
    };

    return multiSelectQuestion;
  }

  throw new Error("unknown question type");
};

export const ZEditFormType = z
  .object({
    formId: z.string().trim().min(1),
    languageCode: z.string().trim().min(1),
    defaultLanguage: z.string().trim().min(1),
    code: z.string().trim().min(1),
    name: ZTranslatedString,
    description: ZTranslatedString.optional(),
    icon: z.string().optional(),
    languages: z.array(z.string()),
    formType: ZFormType.catch(ZFormType.Values.Opening),
    questions: z.array(ZEditQuestionType),
  })
  .superRefine((data, ctx) => {
    if (isNilOrWhitespace(data.name[data.languageCode])) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Form name is required",
        path: ["name"],
      });
    }

    if (
      isNotNilOrWhitespace(data.description?.[data.defaultLanguage]) &&
      isNilOrWhitespace(data.description?.[data.languageCode])
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Form description is required",
        path: ["description"],
      });
    }

    data.questions.forEach((question, index) => {
      if (isNilOrWhitespace(question.text[question.languageCode])) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Question text is required",
          path: ["questions", index, "text"],
        });
      }

      if (
        isNotNilOrWhitespace(question.helptext[question.defaultLanguage]) &&
        isNilOrWhitespace(question.helptext[question.languageCode])
      ) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Question helptext is required",
          path: ["questions", index, "helptext"],
        });
      }

      if (
        question.$questionType === QuestionType.NumberQuestionType ||
        question.$questionType === QuestionType.TextQuestionType
      ) {
        if (
          isNotNilOrWhitespace(
            question.inputPlaceholder[question.defaultLanguage]
          ) &&
          isNilOrWhitespace(question.inputPlaceholder[question.languageCode])
        ) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Input placeholder is required",
            path: ["questions", index, "inputPlaceholder"],
          });
        }
      }

      if (question.$questionType === QuestionType.RatingQuestionType) {
        if (
          isNotNilOrWhitespace(question.lowerLabel[question.defaultLanguage]) &&
          isNilOrWhitespace(question.lowerLabel[question.languageCode])
        ) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Question lower label is required",
            path: ["questions", index, "lowerLabel"],
          });
        }

        if (
          isNotNilOrWhitespace(question.upperLabel[question.defaultLanguage]) &&
          isNilOrWhitespace(question.upperLabel[question.languageCode])
        ) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Question upper label is required",
            path: ["questions", index, "upperLabel"],
          });
        }
      }

      if (
        question.$questionType === QuestionType.SingleSelectQuestionType ||
        question.$questionType === QuestionType.MultiSelectQuestionType
      ) {
        question.options.forEach((option, optionIndex) => {
          if (isNilOrWhitespace(option.text[question.languageCode])) {
            ctx.addIssue({
              code: z.ZodIssueCode.custom,
              message: "Option text is required",
              path: ["questions", index, "options", optionIndex, "text"],
            });
          }
        });

        // check uniqueness of options
        const optionTexts = question.options.map(
          (o) => o.text[question.languageCode]
        );
        const textCountMap = new Map<string | undefined, number>();
        const duplicatedIndexesMap = new Map<string | undefined, number>();

        // Step 1: Count occurrences of each option
        optionTexts.forEach((text, optionIndex) => {
          const numberOfOccurrences = (textCountMap.get(text) || 0) + 1;
          if (numberOfOccurrences > 1) {
            duplicatedIndexesMap.set(text, optionIndex);
          }
          textCountMap.set(text, numberOfOccurrences);
        });

        // Step 2: Mark duplicated options as invalid
        for (const [_, optionIndex] of duplicatedIndexesMap.entries()) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Option text is not unique",
            path: ["questions", index, "options", optionIndex, "text"],
          });
        }
      }

      if (question.hasDisplayLogic) {
        if (question.condition === undefined) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Question condition is required",
            path: ["questions", index, "condition"],
          });
        }

        if (question.parentQuestionId === undefined) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Question parent question is required",
            path: ["questions", index, "parentQuestionId"],
          });
        }

        if (question.value === undefined) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Question value is required",
            path: ["questions", index, "value"],
          });
        }
      }

      return z.NEVER;
    });
  });

export type EditFormType = z.infer<typeof ZEditFormType>;
