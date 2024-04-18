import { BaseQuestion } from '@/common/types';
import QuestionsEdit from './QuestionsEdit';
import PreviewForm from './PreviewForm';
import { useState } from 'react';

export interface FormQuestionsEditorProps {
  languageCode: string;
  localQuestions: BaseQuestion[];
  setLocalQuestions: (questions: BaseQuestion[]) => void;
}

function FormQuestionsEditor({
  languageCode,
  localQuestions,
  setLocalQuestions,
}: FormQuestionsEditorProps) {
  const [activeQuestionId, setActiveQuestionId] = useState<string | undefined>();
  const [invalidQuestions, setInvalidQuestions] = useState<string[]>([]);

  return (
    <div className='flex h-full flex-col'>
      <div className='relative z-0 flex flex-1 overflow-hidden'>
        <main className='relative z-0 flex-1 overflow-y-auto focus:outline-none'>
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
        <aside className='group hidden flex-1 flex-shrink-0 items-center justify-center overflow-hidden border-l border-slate-100 bg-slate-50 py-6  md:flex md:flex-col'>
          <PreviewForm
            languageCode={languageCode}
            localQuestions={localQuestions}
            setActiveQuestionId={setActiveQuestionId}
            activeQuestionId={activeQuestionId}
          />
        </aside>
      </div>
    </div>
  );
}

export default FormQuestionsEditor;
