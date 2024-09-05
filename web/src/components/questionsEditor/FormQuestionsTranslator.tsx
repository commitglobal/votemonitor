import { EditFormType } from '@/features/forms/components/EditForm/EditForm';
import { useEffect, useState } from 'react';
import { useFormContext, useWatch } from 'react-hook-form';
import PreviewQuestion from './preview/PreviewQuestion';
import QuestionsTranslate from './translate/QuestionsTranslate';

function FormQuestionsTranslator() {
  const [activeQuestionId, setActiveQuestionId] = useState<string | undefined>();
  const [questionIndex, setQuestionIndex] = useState<number>(-1);
  const { control } = useFormContext<EditFormType>();

  const questions = useWatch({
    control,
    name: 'questions',
    defaultValue: []
  });

  useEffect(() => {
    setQuestionIndex(questions.findIndex(q=>q.questionId === activeQuestionId));
  }, [activeQuestionId]);

  return (
    <div className='relative z-0 flex flex-1 gap-6 overflow-hidden h-[calc(100%-100px)]'>
      <main className='flex-1 h-full overflow-y-auto bg-slate-50'>
        <QuestionsTranslate
          activeQuestionId={activeQuestionId}
          setActiveQuestionId={setActiveQuestionId}
        />
      </main>
      <aside className='items-center justify-start flex-1 rounded-lg border-slate-100 md:flex md:flex-col'>
        <PreviewQuestion
          activeQuestionId={activeQuestionId}
          setActiveQuestionId={setActiveQuestionId}
          questionIndex={questionIndex}
        />
      </aside>
    </div>
  );
}

export default FormQuestionsTranslator;
