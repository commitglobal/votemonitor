import { BaseAnswer, BaseAnswerSchema, BaseQuestion } from "@/common/types"
import { useEffect, useState } from "react";
import PreviewQuestionFactory from "./PreviewQuestionFactory";
import { Progress } from "../ui/progress";
import { z } from "zod";
import { Button } from "../ui/button";

export interface PreviewFormProps {
    languageCode: string;
    localQuestions: BaseQuestion[];
    activeQuestionId: string | null;
    setActiveQuestionId: (questionId: string) => void;
}

export const ResponseDataSchema = z.record(BaseAnswerSchema);
export type ResponseData = z.infer<typeof ResponseDataSchema>;

function PreviewForm({ languageCode, localQuestions, activeQuestionId, setActiveQuestionId }: PreviewFormProps) {
    const [questionId, setQuestionId] = useState(activeQuestionId || localQuestions[0]?.id);
    const [currentQuestion, setCurrentQuestion] = useState<BaseQuestion | undefined>();
    const [responseData, setResponseData] = useState<ResponseData>({});

    useEffect(() => {
        const id = activeQuestionId || localQuestions[0]?.id;
        setQuestionId(id);
    }, [activeQuestionId, localQuestions]);

    useEffect(() => {
        const currentQuestionIndex = localQuestions.findIndex((q) => q.id === questionId);
        const currentQuestion = localQuestions[currentQuestionIndex];
        setCurrentQuestion(currentQuestion);

    }, [questionId]);

    function resetProgress() {
        setActiveQuestionId(localQuestions[0]?.id!);
        // setQuestionId(localQuestions[0]?.id!);
    }


    function onSubmitAnswer(answer: BaseAnswer) {
        console.log(answer);
        setResponseData({
            ...responseData,
            questionId: answer
        });

        const nextQuestionId = getNextQuestionId(answer.questionId);
        setQuestionId(nextQuestionId);
    }

    function onBackButtonClicked() {

    }

    function getNextQuestionId(questionId: string): string {
        const questions = localQuestions;
        const currentQuestionIndex = localQuestions.findIndex((q) => q.id === questionId);

        if (currentQuestionIndex === -1) throw new Error("Question not found");

        return questions[currentQuestionIndex + 1]?.id || "end";
    }

    return (<div className="flex h-full w-full flex-col justify-between px-6 pb-3 pt-6">
        <Button type="button" onClick={resetProgress}>
            Start from the beggining
        </Button>
        <div>
            {!!currentQuestion ? (
                <PreviewQuestionFactory
                    languageCode={languageCode}
                    question={currentQuestion}
                    answer={responseData[questionId!]!}
                    isFirstQuestion={currentQuestion.id === localQuestions[0]?.id}
                    isLastQuestion={currentQuestion.id === localQuestions[localQuestions.length - 1]?.id}
                    onSubmitAnswer={onSubmitAnswer}
                    onBackButtonClicked={onBackButtonClicked}
                />


            ) : (
                // Handle the case when there are no questions and both welcome and thank you cards are disabled
                <div>No questions available.</div>
            )}
        </div>
        <div className="mt-8">
            <Progress value={33} max={100} />
        </div>
    </div>)
}
export default PreviewForm;
