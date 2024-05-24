import { BaseQuestion } from '@/common/types';
import { useState } from 'react';
import PreviewForm from './PreviewForm';
import QuestionsEdit from './QuestionsEdit';

export interface FormQuestionsEditorProps {
  availableLanguages: string[];
  languageCode: string;
  localQuestions: BaseQuestion[];
  setLocalQuestions: (questions: BaseQuestion[]) => void;
  editLanguageCode?: string;
}

function FormQuestionsEditor({
  availableLanguages,
  languageCode,
  localQuestions,
  editLanguageCode,
  setLocalQuestions,
}: FormQuestionsEditorProps) {
  const [activeQuestionId, setActiveQuestionId] = useState<string | undefined>();
  const [invalidQuestions, setInvalidQuestions] = useState<string[]>([]);

  return (
    <div className='relative z-0 flex flex-1 gap-6 overflow-hidden h-[calc(100%-100px)]'>
      <main className='h-full flex-1 overflow-y-auto bg-slate-50'>
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
      <aside className='flex-1 items-center justify-start  border-slate-100 md:flex md:flex-col rounded-lg'>
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
