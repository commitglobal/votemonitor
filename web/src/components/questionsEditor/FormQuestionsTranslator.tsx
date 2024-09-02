import { EditFormTranslationType } from '@/features/forms/components/EditFormTranslation/EditFormTranslation';
import { EditQuestionType } from '@/features/forms/types';
import { useEffect, useState } from 'react';
import { useFormContext, useWatch } from 'react-hook-form';
import PreviewQuestion from './preview/PreviewQuestion';
import QuestionsTranslate from './translate/QuestionsTranslate';

function FormQuestionsTranslator() {
  const [activeQuestionId, setActiveQuestionId] = useState<string | undefined>();
  const [question, setQuestion] = useState<EditQuestionType | undefined>();
  const [questionIndex, setQuestionIndex] = useState<number>(-1);
  const { control } = useFormContext<EditFormTranslationType>();

  const questions = useWatch({
    control,
    name: 'questions',
    defaultValue: []
  })

  const languageCode = useWatch({
    control,
    name: 'languageCode'
  })

  useEffect(() => {
    setQuestion(questions.find(q=>q.questionId === activeQuestionId));
    setQuestionIndex(questions.findIndex(q=>q.questionId === activeQuestionId));
  }, [activeQuestionId]);

  return (
    <div className='relative z-0 flex flex-1 gap-6 overflow-hidden h-[calc(100%-100px)]'>
      <main className='h-full flex-1 overflow-y-auto bg-slate-50'>
        <QuestionsTranslate
          activeQuestionId={activeQuestionId}
          setActiveQuestionId={setActiveQuestionId}
        />
      </main>
      <aside className='flex-1 items-center justify-start  border-slate-100 md:flex md:flex-col rounded-lg'>
        <PreviewQuestion
          activeQuestionId={activeQuestionId}
          setActiveQuestionId={setActiveQuestionId}
          languageCode={languageCode}
          question={question}
          questionIndex={questionIndex}
          questions={questions}
        />
      </aside>
    </div>
  );
}

export default FormQuestionsTranslator;
