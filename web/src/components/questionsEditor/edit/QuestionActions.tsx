import { ArrowUpIcon, ArrowDownIcon, TrashIcon, DocumentDuplicateIcon } from "@heroicons/react/24/solid";

import { MoveDirection } from "../QuestionsEdit";

export interface QuestionActionsProps {
    questionIdx: number;
    isLastQuestion: boolean;
    duplicateQuestion: (questionIndex: number) => void;
    deleteQuestion: (questionIndex: number) => void;
    moveQuestion: (questionIndex: number, direction: MoveDirection) => void;

}

function QuestionActions({ questionIdx, isLastQuestion, duplicateQuestion, deleteQuestion, moveQuestion }: QuestionActionsProps) {
    return (
        <div className="flex space-x-4">
            <ArrowUpIcon
                className={`h-4 cursor-pointer text-slate-500 hover:text-slate-600 ${questionIdx === 0 ? "opacity-50" : ""
                    }`}
                onClick={(e) => {
                    if (questionIdx !== 0) {
                        e.stopPropagation();
                        moveQuestion(questionIdx, MoveDirection.UP);
                    }
                }}
            />
            <ArrowDownIcon
                className={`h-4 cursor-pointer text-slate-500 hover:text-slate-600 ${isLastQuestion ? "opacity-50" : ""
                    }`}
                onClick={(e) => {
                    if (!isLastQuestion) {
                        e.stopPropagation();
                        moveQuestion(questionIdx, MoveDirection.DOWN);
                    }
                }}
            />
            <DocumentDuplicateIcon
                className="h-4 cursor-pointer text-slate-500 hover:text-slate-600"
                onClick={(e) => {
                    e.stopPropagation();
                    duplicateQuestion(questionIdx);
                }}
            />
            <TrashIcon
                className="h-4 cursor-pointer text-slate-500 hover:text-slate-600"
                onClick={(e) => {
                    e.stopPropagation();
                    deleteQuestion(questionIdx);
                }}
            />
        </div>
    )
}

export default QuestionActions
