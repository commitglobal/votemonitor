import { BaseAnswer } from "@/common/types";
import { create } from "zustand";

export type FormEditorAnswersContextValue = {
  answers: {
    [questionId: string]: BaseAnswer | undefined;
  };
  setAnswer: (answer: BaseAnswer | undefined) => void;
  getAnswer: (questionId: string) => BaseAnswer | undefined;
  resetForm: () => void
};

export const useFormAnswersStore = create<FormEditorAnswersContextValue>(
  (set, get) => ({
    answers: {},
    setAnswer: (answer: BaseAnswer | undefined) =>
      set((state) => ({
        ...state,
        answers: {
          ...state.answers,
          [answer?.questionId!]: answer,
        },
      })),
    getAnswer: (questionId: string) => get().answers[questionId],

    resetForm: () =>
      set((state) => ({
        ...state,
        answers: {},
      })),
  })
);
