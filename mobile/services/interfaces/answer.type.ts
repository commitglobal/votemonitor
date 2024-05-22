import { FormQuestionType } from "./question.type";

export type ApiFormAnswerType =
  | "textAnswer"
  | "numberAnswer"
  | "dateAnswer"
  | "singleSelectAnswer"
  | "multiSelectAnswer"
  | "ratingAnswer";

export type ApiFormAnswer =
  | TextAnswer
  | NumberAnswer
  | RatingAnswer
  | DateAnswer
  | SingleSelectAnswer
  | MultiSelectAnswer;

export type TextAnswer = {
  questionId: string;
  $answerType: "textAnswer";
  text: string;
};

export type NumberAnswer = {
  questionId: string;
  $answerType: "numberAnswer";
  value: number;
};

export type RatingAnswer = {
  questionId: string;
  $answerType: "ratingAnswer";
  value: number;
};

export type DateAnswer = {
  questionId: string;
  $answerType: "dateAnswer";
  date: string; // ISO String
};

export type SingleSelectAnswer = {
  questionId: string;
  $answerType: "singleSelectAnswer";
  selection: {
    optionId: string;
    text?: string;
  };
};

export type MultiSelectAnswer = {
  questionId: string;
  $answerType: "multiSelectAnswer";
  selection: {
    optionId: string;
    text?: string;
  }[];
};

export const FormQuestionAnswerTypeMapping: Record<FormQuestionType, ApiFormAnswerType> = {
  numberQuestion: "numberAnswer",
  textQuestion: "textAnswer",
  dateQuestion: "dateAnswer",
  singleSelectQuestion: "singleSelectAnswer",
  multiSelectQuestion: "multiSelectAnswer",
  ratingQuestion: "ratingAnswer",
};
