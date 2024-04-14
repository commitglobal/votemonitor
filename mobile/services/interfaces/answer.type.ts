import { FormQuestionType } from "./question.type";

export type ApiFormAnswerType =
  | "textAnswer"
  | "numberAnswer"
  | "dateAnswer"
  | "singleSelectAnswer"
  | "multiSelectAnswer"
  | "ratingAnswer";

export type ApiFormAnswer = {
  questionId: string;
} & (
  | {
      $answerType: "textAnswer";
      Text: string;
    }
  | {
      $answerType: "numberAnswer" | "ratingAnswer";
      value: string;
    }
  | {
      $answerType: "dateAnswer";
      Date: string; // ISO String
    }
  | {
      $answerType: "singleSelectAnswer";
      selection: {
        optionId: string;
        text?: string;
      };
    }
  | {
      $answerType: "multiSelectAnswer";
      selection: {
        optionId: string;
        text?: string;
      }[];
    }
);

export const FormQuestionAnswerTypeMapping: Record<FormQuestionType, ApiFormAnswerType> = {
  numberQuestion: "numberAnswer",
  textQuestion: "textAnswer",
  dateQuestion: "dateAnswer",
  singleSelectQuestion: "singleSelectAnswer",
  multiSelectQuestion: "multiSelectAnswer",
  ratingQuestion: "ratingAnswer",
};
