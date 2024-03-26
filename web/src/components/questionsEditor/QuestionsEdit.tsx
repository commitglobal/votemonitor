import { BaseQuestion } from "@/common/types"
``
export interface QuestionsEditProps {
  localQuestions: BaseQuestion[];
  setLocalQuestions: (questions: BaseQuestion[]) => void;
  activeQuestionId: string | null;
  setActiveQuestionId: (questionId: string) => void;
  invalidQuestions: string[] | null;
  setInvalidQuestions: (questions: string[]) => void;
}

function QuestionsEdit({ localQuestions, setLocalQuestions, activeQuestionId, setActiveQuestionId, invalidQuestions, setInvalidQuestions }: QuestionsEditProps) {

  return (<div className="flex h-full w-full flex-col justify-between px-6 pb-3 pt-6">
    <div>
      {localQuestions.length === 0 ? (
        // Handle the case when there are no questions and both welcome and thank you cards are disabled
        <div>No questions available.</div>
      ) : (
        <div>QuestionsEdit</div>
      )}
    </div>
  </div>)
}
export default QuestionsEdit;