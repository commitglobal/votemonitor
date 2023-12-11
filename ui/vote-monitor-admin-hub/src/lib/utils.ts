import { FormModel } from "@/redux/api/types";
import { ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}


export const calculateElementIdx = (form: FormModel, currentQustionIdx: number): number => {
  const currentQuestion = form.questions[currentQustionIdx];
  const formLength = form.questions.length;
  const middleIdx = Math.floor(formLength / 2);
  const possibleNextQuestions = currentQuestion?.logic?.map((l) => l.destination) || [];

  const getLastQuestionIndex = () => {
    const lastQuestion = form.questions
      .filter((q) => possibleNextQuestions.includes(q.id))
      .sort((a, b) => form.questions.indexOf(a) - form.questions.indexOf(b))
      .pop();
    return form.questions.findIndex((e) => e.id === lastQuestion?.id);
  };

  let elementIdx = currentQustionIdx || 0.5;
  const lastprevQuestionIdx = getLastQuestionIndex();

  if (lastprevQuestionIdx > 0) elementIdx = Math.min(middleIdx, lastprevQuestionIdx - 1);
  if (possibleNextQuestions.includes("end")) elementIdx = middleIdx;
  return elementIdx;
};
