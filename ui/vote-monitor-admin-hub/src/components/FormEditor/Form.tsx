import { cn } from "@/lib/utils";
import { TResponseData, TResponseUpdate } from "@/redux/api/responses";
import { FormModel } from "@/redux/api/types";
import { useEffect, useRef, useState } from "react";
import ProgressBar from "./ProgressBar";
import { evaluateCondition } from "@/lib/logicEvaluator";
import QuestionConditional from "./QuestionConditional";


export interface FormBaseProps {
  form: FormModel;
  activeQuestionId?: string;
  onDisplay?: () => void;
  onResponse?: (response: TResponseUpdate) => void;
  onFinished?: () => void;
  onActiveQuestionChange?: (questionId: string) => void;
  autoFocus?: boolean;
  prefillResponseData?: TResponseData;
  responseCount?: number;
}

export function Form({
  form,
  activeQuestionId,
  onDisplay = () => { },
  onActiveQuestionChange = () => { },
  onResponse = () => { },
  onFinished = () => { },
  prefillResponseData,
  responseCount,
}: FormBaseProps) {
  const [questionId, setQuestionId] = useState(
    activeQuestionId || form?.questions[0]?.id
  );
  const [loadingElement, setLoadingElement] = useState(false);
  const [history, setHistory] = useState<string[]>([]);
  const [responseData, setResponseData] = useState<TResponseData>({});
  const currentQuestionIndex = form.questions.findIndex((q) => q.id === questionId);
  const currentQuestion = form.questions[currentQuestionIndex];
  const contentRef = useRef<HTMLDivElement | null>(null);
  useEffect(() => {
    setQuestionId(activeQuestionId || form?.questions[0]?.id);
  }, [activeQuestionId, form.questions]);

  useEffect(() => {
    // scroll to top when question changes
    if (contentRef.current) {
      contentRef.current.scrollTop = 0;
    }
  }, [questionId]);

  useEffect(() => {
    // call onDisplay when component is mounted
    onDisplay();
    if (prefillResponseData) {
      onSubmit(prefillResponseData, true);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
  let currIdx = currentQuestionIndex;
  let currQues = currentQuestion;
  function getNextQuestionId(data: TResponseData, isFromPrefilling: Boolean = false): string {
    const questions = form.questions;
    const responseValue = data[questionId];

    if (currIdx === -1) throw new Error("Question not found");

    if (currQues?.logic && currQues?.logic.length > 0) {
      for (let logic of currQues.logic) {
        if (!logic.destination) continue;

        if (evaluateCondition(logic, responseValue)) {
          return logic.destination;
        }
      }
    }
    return questions[currIdx + 1]?.id || "end";
  }

  const onChange = (responseDataUpdate: TResponseData) => {
    const updatedResponseData = { ...responseData, ...responseDataUpdate };
    setResponseData(updatedResponseData);
  };

  const onSubmit = (responseData: TResponseData, isFromPrefilling: Boolean = false) => {
    const questionId = Object.keys(responseData)[0];
    setLoadingElement(true);
    const nextQuestionId = getNextQuestionId(responseData, isFromPrefilling);
    const finished = nextQuestionId === "end";
    onResponse({ data: responseData, finished });
    if (finished) {
      onFinished();
    }
    setQuestionId(nextQuestionId);
    // add to history
    setHistory([...history, questionId]);
    setLoadingElement(false);
    onActiveQuestionChange(nextQuestionId);
  };

  const onBack = (): void => {
    let prevQuestionId;
    // use history if available
    if (history?.length > 0) {
      const newHistory = [...history];
      prevQuestionId = newHistory.pop();
      if (prefillResponseData && prevQuestionId === form.questions[0].id) return;
      setHistory(newHistory);
    } else {
      // otherwise go back to previous question in array
      prevQuestionId = form.questions[currIdx - 1]?.id;
    }
    if (!prevQuestionId) throw new Error("Question not found");
    setQuestionId(prevQuestionId);
    onActiveQuestionChange(prevQuestionId);
  };
  function getCardContent() {
    const currQues = form.questions.find((q) => q.id === questionId);
    return (
      currQues && (
        <QuestionConditional
          formId={form.id}
          question={currQues}
          value={responseData[currQues.id]}
          onChange={onChange}
          onSubmit={onSubmit}
          onBack={onBack}
          isFirstQuestion={
            history && prefillResponseData
              ? history[history.length - 1] === form.questions[0].id
              : currQues.id === form?.questions[0]?.id
          }
          isLastQuestion={currQues.id === form.questions[form.questions.length - 1].id}
        />
      )
    );
  }

  return (
    <>
      <div className="flex h-full w-full flex-col justify-between px-6 pb-3 pt-6">
        <div ref={contentRef} className={cn(loadingElement ? "animate-pulse opacity-60" : "", "my-auto")}>
          {form.questions.length === 0 ? (
            // Handle the case when there are no questions and both welcome and thank you cards are disabled
            <div>No questions available.</div>
          ) : (
            getCardContent()
          )}
        </div>
        <div className="mt-8">
          <ProgressBar form={form} questionId={questionId} />
        </div>
      </div>
    </>
  );
}


