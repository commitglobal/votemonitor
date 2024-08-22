import { useEffect, useState } from 'react';
import QuestionsEdit from './edit/QuestionsEdit';
import PreviewQuestion from './preview/PreviewQuestion';
import { EditFormType } from '@/features/forms/components/EditForm/EditForm';
import { useFormContext, useWatch } from 'react-hook-form';
import { EditQuestionType } from '@/features/forms/types';

function FormQuestionsEditor() {
  const [activeQuestionId, setActiveQuestionId] = useState<string | undefined>();
  const [question, setQuestion] = useState<EditQuestionType | undefined>();
  const [questionIndex, setQuestionIndex] = useState<number>(-1);
  const { control } = useFormContext<EditFormType>();
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
        <QuestionsEdit
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

export default FormQuestionsEditor;
