import { FormModel } from "@/redux/api/types";
import { Progress } from "../ui/progress";
import { useCallback, useMemo } from "react";
import { calculateElementIdx } from "@/lib/utils";

interface ProgressBarProps {
  form: FormModel;
  questionId: string;
}

export default function ProgressBar({ form, questionId }: ProgressBarProps) {
  const currentQuestionIdx = useMemo(
    () => form.questions.findIndex((e) => e.id === questionId),
    [form, questionId]
  );

  const calculateProgress = useCallback((questionId: string, form: FormModel, progress: number) => {
    if (form.questions.length === 0) return 0;
    let currentQustionIdx = form.questions.findIndex((e) => e.id === questionId);
    if (currentQustionIdx === -1) currentQustionIdx = 0;
    const elementIdx = calculateElementIdx(form, currentQustionIdx);

    const newProgress = elementIdx / form.questions.length;
    let updatedProgress = progress;
    if (newProgress > progress) {
      updatedProgress = newProgress;
    } else if (newProgress <= progress && progress + 0.1 <= 1) {
      updatedProgress = progress + 0.1;
    }
    return updatedProgress;
  }, []);

  const progressArray = useMemo(() => {
    let progress = 0;
    let progressArrayTemp: number[] = [];
    form.questions.forEach((question) => {
      progress = calculateProgress(question.id, form, progress);
      progressArrayTemp.push(progress*100);
    });

    return progressArrayTemp;
  }, [calculateProgress, form]);

  return <Progress value={questionId === "end" ? 100 : progressArray[currentQuestionIdx]} />;
}
