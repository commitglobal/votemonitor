export type FormQuestionType =
  | "numberQuestion"
  | "textQuestion"
  | "dateQuestion"
  | "timeQuestion"
  | "dateTimeQuestion"
  | "singleSelectQuestion"
  | "multiSelectQuestion"
  | "ratingQuestion";

export type ApiFormQuestionSelectOption = {
  id: string;
  text: Record<string, string>;
  isFlagged: boolean;
  isFreeText: boolean;
};

export type DisplayLogicCondition =
  | "Equals"
  | "NotEquals"
  | "LessThan"
  | "LessEqual"
  | "GreaterThan"
  | "GreaterEqual"
  | "Includes"
  | "AnyOf"
  | "All";

type ApiFormQuestionDisplayLogic = {
  condition: DisplayLogicCondition;
  parentQuestionId: string;
  // number/rating/text parents compare against value
  value?: string | null;
  // select parents ("AnyOf" / "All") list their options here; value is empty
  optionIds?: string[] | null;
};

export type ApiFormQuestion = {
  id: string; // "f5cc674f-48b3-4918-8f9e-67dec35f1009";
  code: string; // "A2"; // A - deschidere, B - in timpul zilei, C - la numarare
  text: Record<string, string>; // { EN: string; // "mood"; RO: string; // "dispozitie";};
  helptext: Record<string, string>; // { EN: string; // "mood"; RO: string; // "dispozitie";};
  displayLogic: ApiFormQuestionDisplayLogic | null;
} & (
  | {
      $questionType: "textQuestion" | "numberQuestion";
      inputPlaceholder: Record<string, string>; // { EN: string; // "mood"; RO: string; // "dispozitie";};
    }
  | {
      $questionType: "ratingQuestion";
      scale: "OneTo3" | "OneTo4" | "OneTo5" | "OneTo6" | "OneTo7" | "OneTo8" | "OneTo9" | "OneTo10";
      lowerLabel?: Record<string, string>; // { EN: string; // "mood"; RO: string; // "dispozitie";};
      upperLabel?: Record<string, string>; // { EN: string; // "mood"; RO: string; // "dispozitie";};
    }
  | {
      $questionType: "multiSelectQuestion" | "singleSelectQuestion";
      options: ApiFormQuestionSelectOption[];
    }
  | {
      // dateQuestion: a calendar day; timeQuestion: a time of day; dateTimeQuestion: a moment
      $questionType: "dateQuestion" | "timeQuestion" | "dateTimeQuestion";
    }
);
