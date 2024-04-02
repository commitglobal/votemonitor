import { BaseQuestion } from "@/common/types"
import QuestionsEdit from "./QuestionsEdit";
import PreviewForm from "./PreviewForm";

export interface FormQuestionsEditorProps {
  languageCode: string;
  localQuestions: BaseQuestion[];
  setLocalQuestions: (questions: BaseQuestion[]) => void;
  activeQuestionId: string | undefined;
  setActiveQuestionId: (questionId: string | undefined) => void;
  invalidQuestions: string[] | null
  setInvalidQuestions: (questions: string[]) => void;
}

function FormQuestionsEditor({
  languageCode,
  localQuestions,
  setLocalQuestions,
  activeQuestionId,
  setActiveQuestionId,
  invalidQuestions,
  setInvalidQuestions }: FormQuestionsEditorProps) {

  return (
    (
      <div className="flex h-full flex-col">
        <div className="relative z-0 flex flex-1 overflow-hidden">
          <main className="relative z-0 flex-1 overflow-y-auto focus:outline-none">
            <QuestionsEdit
              languageCode={languageCode}
              localQuestions={localQuestions}
              setLocalQuestions={setLocalQuestions}
              activeQuestionId={activeQuestionId}
              setActiveQuestionId={setActiveQuestionId}
              invalidQuestions={invalidQuestions}
              setInvalidQuestions={setInvalidQuestions}
            />

          </main>
          <aside className="group hidden flex-1 flex-shrink-0 items-center justify-center overflow-hidden border-l border-slate-100 bg-slate-50 py-6  md:flex md:flex-col">
            <PreviewForm
              languageCode={languageCode}
              localQuestions={localQuestions}
              setActiveQuestionId={setActiveQuestionId}
              activeQuestionId={activeQuestionId}
            />
          </aside>
        </div>
      </div>)
  )
}

export default FormQuestionsEditor
