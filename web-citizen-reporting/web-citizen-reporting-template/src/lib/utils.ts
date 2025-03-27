import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

import {
  DateAnswerSchema,
  MultiSelectAnswerSchema,
  NumberAnswerSchema,
  QuestionType,
  RatingAnswerSchema,
  SingleSelectAnswerSchema,
  TextAnswerSchema,
  type BaseAnswer,
  type BaseQuestion,
  type DateAnswer,
  type MultiSelectAnswer,
  type MultiSelectQuestion,
  type NumberAnswer,
  type RatingAnswer,
  type RatingQuestion,
  type SingleSelectAnswer,
  type SingleSelectQuestion,
  type TextAnswer,
  type TextQuestion,
  type NumberQuestion,
  type DateQuestion,
} from "@/common/types";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const isProduction = import.meta.env.MODE === "production";
export const electionRoundId = import.meta.env.VITE_ELECTION_ROUND_ID;

export function isDateAnswer(answer: BaseAnswer): answer is DateAnswer {
  return DateAnswerSchema.safeParse(answer).success;
}

export function isDateQuestion(
  question: BaseQuestion
): question is DateQuestion {
  return question.$questionType === QuestionType.DateQuestionType;
}

export function isMultiSelectAnswer(
  answer: BaseAnswer
): answer is MultiSelectAnswer {
  return MultiSelectAnswerSchema.safeParse(answer).success;
}

export function isMultiSelectQuestion(
  question: BaseQuestion
): question is MultiSelectQuestion {
  return question.$questionType === QuestionType.MultiSelectQuestionType;
}

export function isNumberAnswer(answer: BaseAnswer): answer is NumberAnswer {
  return NumberAnswerSchema.safeParse(answer).success;
}

export function isNumberQuestion(
  question: BaseQuestion
): question is NumberQuestion {
  return question.$questionType === QuestionType.NumberQuestionType;
}

export function isRatingAnswer(answer: BaseAnswer): answer is RatingAnswer {
  return RatingAnswerSchema.safeParse(answer).success;
}

export function isRatingQuestion(
  question: BaseQuestion
): question is RatingQuestion {
  return question.$questionType === QuestionType.RatingQuestionType;
}

export function isSingleSelectAnswer(
  answer: BaseAnswer
): answer is SingleSelectAnswer {
  return SingleSelectAnswerSchema.safeParse(answer).success;
}

export function isSingleSelectQuestion(
  question: BaseQuestion
): question is SingleSelectQuestion {
  return question.$questionType === QuestionType.SingleSelectQuestionType;
}

export function isTextAnswer(answer: BaseAnswer): answer is TextAnswer {
  return TextAnswerSchema.safeParse(answer).success;
}

export function isTextQuestion(
  question: BaseQuestion
): question is TextQuestion {
  return question.$questionType === QuestionType.TextQuestionType;
}
