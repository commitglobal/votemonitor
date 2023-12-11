

import { useGetFormQuery } from '@/redux/api/formsApi';
import { FormModel } from '@/redux/api/types';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import FormMenuBar from './FormMenuBar';
import QuestionsView from './QuestionsView';
import PreviewForm from './PreviewForm';

function FormEditor() {
  const { formId } = useParams();
  const { data: form, isLoading } = useGetFormQuery(formId!);
  const [localForm, setLocalForm] = useState<FormModel | null>();
  const [activeQuestionId, setActiveQuestionId] = useState<string | null>(null);
  const [invalidQuestions, setInvalidQuestions] = useState<string[] | null>(null);

  useEffect(() => {
    if (form) {
      if (localForm) return;
      setLocalForm(JSON.parse(JSON.stringify(form)));

      if (form.questions.length > 0) {
        setActiveQuestionId(form.questions[0].id);
      }
    }
  }, [formId, form]);


  return (
    <>
      {isLoading || !localForm ? <div>loading</div> :
        (
          <div className="flex h-full flex-col">
            <FormMenuBar
              form={form!}
              localForm={localForm}
              setLocalForm={setLocalForm}
              setInvalidQuestions={setInvalidQuestions}
              responseCount={0}
            />
            <div className="relative z-0 flex flex-1 overflow-hidden">
              <main className="relative z-0 flex-1 overflow-y-auto focus:outline-none">
                <QuestionsView
                  localForm={localForm}
                  setLocalForm={setLocalForm}
                  activeQuestionId={activeQuestionId}
                  setActiveQuestionId={setActiveQuestionId}
                  invalidQuestions={invalidQuestions}
                  setInvalidQuestions={setInvalidQuestions}
                />

              </main>
              <aside className="group hidden flex-1 flex-shrink-0 items-center justify-center overflow-hidden border-l border-slate-100 bg-slate-50 py-6  md:flex md:flex-col">
                <PreviewForm
                  form={localForm}
                  setActiveQuestionId={setActiveQuestionId}
                  activeQuestionId={activeQuestionId}
                />
              </aside>
            </div>
          </div>)}
    </>
  )
}

export default FormEditor


