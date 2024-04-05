export type ApiFormQuestionType =
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
