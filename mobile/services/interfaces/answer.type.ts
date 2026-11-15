import { FormQuestionType } from "./question.type";

export type ApiFormAnswerType =
  | "textAnswer"
  | "numberAnswer"
  | "dateAnswer"
  | "timeAnswer"
  | "dateTimeAnswer"
  | "singleSelectAnswer"
  | "multiSelectAnswer"
  | "ratingAnswer";

export type ApiFormAnswer =
  | TextAnswer
  | NumberAnswer
  | RatingAnswer
  | DateAnswer
  | TimeAnswer
  | DateTimeAnswer
  | SingleSelectAnswer
  | MultiSelectAnswer;

// When the answer was first given and last changed on the device (ISO). Optional: older answers have neither.
type AnswerTimestamps = {
  createdAt?: string;
  lastUpdatedAt?: string;
};

export type TextAnswer = AnswerTimestamps & {
  questionId: string;
  $answerType: "textAnswer";
  text: string;
};

export type NumberAnswer = AnswerTimestamps & {
  questionId: string;
  $answerType: "numberAnswer";
  value: number;
};

export type RatingAnswer = AnswerTimestamps & {
  questionId: string;
  $answerType: "ratingAnswer";
  value: number;
};

export type DateAnswer = AnswerTimestamps & {
  questionId: string;
  $answerType: "dateAnswer";
  date: string; // the picked day as "YYYY-MM-DDT00:00:00Z"; only the date part counts
};

export type TimeAnswer = AnswerTimestamps & {
  questionId: string;
  $answerType: "timeAnswer";
  time: string; // "HH:mm:ss", a wall-clock time without a date or time zone
};

export type DateTimeAnswer = AnswerTimestamps & {
  questionId: string;
  $answerType: "dateTimeAnswer";
  dateTime: string; // ISO String
};

export type SingleSelectAnswer = AnswerTimestamps & {
  questionId: string;
  $answerType: "singleSelectAnswer";
  selection: {
    optionId: string;
    text?: string;
    value?: string;
  };
};

export type MultiSelectAnswer = AnswerTimestamps & {
  questionId: string;
  $answerType: "multiSelectAnswer";
  selection: {
    optionId: string;
    text?: string;
  }[];
  selectionValues?: string[];
};

export const FormQuestionAnswerTypeMapping: Record<FormQuestionType, ApiFormAnswerType> = {
  numberQuestion: "numberAnswer",
  textQuestion: "textAnswer",
  dateQuestion: "dateAnswer",
  timeQuestion: "timeAnswer",
  dateTimeQuestion: "dateTimeAnswer",
  singleSelectQuestion: "singleSelectAnswer",
  multiSelectQuestion: "multiSelectAnswer",
  ratingQuestion: "ratingAnswer",
};
