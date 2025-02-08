import { EditFormType } from '@/components/FormEditor/FormEditor';
import { useFieldArray, useFormContext } from 'react-hook-form';
import TranslateQuestionFactory from './TranslateQuestionFactory';

export interface QuestionsTranslateProps {
  activeQuestionId: string | undefined;
  setActiveQuestionId: (questionId: string | undefined) => void;
}

function QuestionsTranslate({
  activeQuestionId,
  setActiveQuestionId
}: QuestionsTranslateProps) {
  const { control } = useFormContext<EditFormType>();

  const { fields } = useFieldArray({
    name: "questions",
    control: control,
  });

  return (
    <div className='mb-5 grid grid-cols-1 gap-5 '>
      <div className='grid gap-5'>
        {fields.map((field, questionIndex) => (
          <TranslateQuestionFactory
            key={field.id}
            questionIndex={questionIndex}
            activeQuestionId={activeQuestionId}
            setActiveQuestionId={setActiveQuestionId}
          />
        ))}
      </div>
    </div>
  );
}
export default QuestionsTranslate;
