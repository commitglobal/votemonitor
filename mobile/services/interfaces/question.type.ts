export type FormQuestionType =
  | "numberQuestion"
  | "textQuestion"
  | "dateQuestion"
  | "singleSelectQuestion"
  | "multiSelectQuestion"
  | "ratingQuestion";

export type ApiFormQuestionSelectOption = {
  id: string;
  text: Record<string, string>;
  isFlagged: boolean;
  isFreeText: boolean;
};

export type ApiFormQuestion = {
  id: string; // "f5cc674f-48b3-4918-8f9e-67dec35f1009";
  code: string; // "A2"; // A - deschidere, B - in timpul zilei, C - la numarare
  text: Record<string, string>; // { EN: string; // "mood"; RO: string; // "dispozitie";};
  helptext: Record<string, string>; // { EN: string; // "mood"; RO: string; // "dispozitie";};
} & (
  | {
      $questionType: "textQuestion" | "numberQuestion";
      inputPlaceholder: Record<string, string>; // { EN: string; // "mood"; RO: string; // "dispozitie";};
    }
  | {
      $questionType: "ratingQuestion";
      scale: string; // "OneTo10" //TODO: ce posibilitati mai sunt?
    }
  | {
      $questionType: "multiSelectQuestion" | "singleSelectQuestion";
      options: ApiFormQuestionSelectOption[];
    }
  | {
      $questionType: "dateQuestion";
    }
);
