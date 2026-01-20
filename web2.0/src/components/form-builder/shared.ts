import { DisplayLogicCondition, FormType, QuestionType, RatingScaleType } from "@/types/form";
import { Language } from "@/types/language";
import { formOptions } from "@tanstack/react-form";
import * as z from "zod";

const baseQuestionSchema = z.object({
    questionId: z.string(),
    text: z.string().trim().min(1),
    helptext: z.string().trim().optional(),
  
    hasDisplayLogic: z.boolean().catch(false),
    parentQuestionId: z.string().optional(),
    condition: z.enum(DisplayLogicCondition).optional().catch(DisplayLogicCondition.Equals),
    value: z.string().optional(),
  
    code: z.string().trim().min(1),
  });
  
  const ratingQuestionSchema = baseQuestionSchema.extend({
    $questionType: z.literal(QuestionType.RatingQuestionType),
    lowerLabel: z.string().trim().optional(),
    upperLabel: z.string().trim().optional(),
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
  
  const dateQuestionSchema = baseQuestionSchema.extend({
    $questionType: z.literal(QuestionType.DateQuestionType),
  });
  
  const numberQuestionSchema = baseQuestionSchema.extend({
    $questionType: z.literal(QuestionType.NumberQuestionType),
    inputPlaceholder: z.string().trim().optional(),
  });
  
  export const textQuestionSchema = baseQuestionSchema.extend({
    $questionType: z.literal(QuestionType.TextQuestionType),
    inputPlaceholder: z.string().trim().optional(),
  });
  
  const selectOptionSchema = z.object({
    optionId: z.string(),
    text: z.string().trim().min(1),
    isFlagged: z.boolean(),
    isFreeText: z.boolean(),
  });
  
  const multiSelectQuestionSchema = baseQuestionSchema.extend({
    $questionType: z.literal(QuestionType.MultiSelectQuestionType),
    options: z.array(selectOptionSchema).min(1),
  });
  
  const singleSelectQuestionSchema = baseQuestionSchema.extend({
    $questionType: z.literal(QuestionType.SingleSelectQuestionType),
    options: z.array(selectOptionSchema).min(1),
  });
  
  export const questionSchema = z.discriminatedUnion('$questionType', [
    textQuestionSchema,
    numberQuestionSchema,
    dateQuestionSchema,
    ratingQuestionSchema,
    singleSelectQuestionSchema,
    multiSelectQuestionSchema,
  ]);


  export const formEditSchema = z
  .object({
    formType: z.enum(FormType),
    defaultLanguage: z.enum(Language),
    code: z.string().trim().min(1, 'Code is required.'),
    name: z.string().trim().min(1, 'Name is required.'),
    description: z.string().trim(),
    icon: z.string().or(z.undefined()),
    languages: z.array(z.enum(Language)).nonempty('At least one language is required.'),
    questions: z.array(questionSchema),
  });


  const formEditDefaultValues: z.infer<typeof formEditSchema> = {
    formType: FormType.Opening,
    defaultLanguage: Language.EN,
    code: "",
    name: "",
    description: "",
    icon: "",
    languages: [Language.EN],
    questions: [],
  };



  export const formEditOpts = formOptions({
    defaultValues: formEditDefaultValues,
  });
