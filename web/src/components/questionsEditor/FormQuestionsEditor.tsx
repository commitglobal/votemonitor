import { BaseQuestion } from '@/common/types';
import QuestionsEdit from './QuestionsEdit';
import PreviewForm from './PreviewForm';
import { useState } from 'react';

export interface FormQuestionsEditorProps {
  availableLanguages: string[];
  languageCode: string;
  localQuestions: BaseQuestion[];
  setLocalQuestions: (questions: BaseQuestion[]) => void;
}

function FormQuestionsEditor({
  availableLanguages,
  languageCode,
  localQuestions,
  setLocalQuestions,
}: FormQuestionsEditorProps) {
  const [activeQuestionId, setActiveQuestionId] = useState<string | undefined>();
  const [invalidQuestions, setInvalidQuestions] = useState<string[]>([]);

  return (
    <div className='flex w-full gap-4'>
      <main className='question-list flex-1'>
        <QuestionsEdit
          availableLanguages={availableLanguages}
          languageCode={languageCode}
          localQuestions={localQuestions}
          setLocalQuestions={setLocalQuestions}
          activeQuestionId={activeQuestionId}
          setActiveQuestionId={setActiveQuestionId}
          invalidQuestions={invalidQuestions}
          setInvalidQuestions={setInvalidQuestions}
        />
      </main>
      <aside className='flex-1 items-center justify-center  border-slate-100 bg-slate-50 py-6  md:flex md:flex-col rounded-lg'>
        <PreviewForm
          languageCode={languageCode}
          localQuestions={localQuestions}
          setActiveQuestionId={setActiveQuestionId}
          activeQuestionId={activeQuestionId}
        />
      </aside>
    </div>
  );
}

export default FormQuestionsEditor;
