import { BaseQuestion, newTranslatedString, NumberQuestion } from '@/common/types';
import { Label } from '@/components/ui/label';
import { useTranslation } from 'react-i18next';

import { Input } from '../../ui/input';
import DisplayLogicEditor from './DisplayLogicEditor';
import QuestionHeader from './QuestionHeader';
import { useParams } from '@tanstack/react-router';

export interface EditNumberQuestionProps {
  formQuestions: BaseQuestion[];
  languageCode: string;
  availableLanguages: string[];
  questionIdx: number;
  isInValid: boolean;
  question: NumberQuestion;
  updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
}

function EditNumberQuestion({
  formQuestions,
  availableLanguages,
  languageCode,
  questionIdx,
  isInValid,
  question,
  updateQuestion,
}: EditNumberQuestionProps) {
  const { t } = useTranslation();

  const params: any = useParams({
    strict: false,
  });

  function updateInputPlaceholder(inputPlaceholder: string) {
    const updatedInputPlaceholder = question.inputPlaceholder
      ? {
          ...question.inputPlaceholder,
          [params.languageCode ? params.languageCode : languageCode]: inputPlaceholder,
        }
      : newTranslatedString(availableLanguages, languageCode);

    const updatedNumberQuestion: NumberQuestion = { ...question, inputPlaceholder: updatedInputPlaceholder };
    updateQuestion(questionIdx, updatedNumberQuestion);
  }

  return (
    <div>
      <QuestionHeader
        availableLanguages={availableLanguages}
        languageCode={languageCode}
        isInValid={isInValid}
        question={question}
        questionIdx={questionIdx}
        updateQuestion={updateQuestion}
      />

      <div className='mt-3'>
        <Label htmlFor='inputPlaceholder'>{t('questionEditor.question.inputPlaceholder')}</Label>
        <div className='mt-2 flex flex-col gap-6'>
          <div className='flex items-center space-x-2'>
            <Input
              id='inputPlaceholder'
              name='inputPlaceholder'
              value={
                params['languageCode']
                  ? question.inputPlaceholder?.[params['languageCode']]
                  : question.inputPlaceholder?.[languageCode]
              }
              placeholder={params['languageCode'] ? question.inputPlaceholder?.[languageCode] : ''}
              onChange={(e) => updateInputPlaceholder(e.target.value)}
              className={
                isInValid && !!question.inputPlaceholder && question.inputPlaceholder[languageCode]!.trim() === ''
                  ? 'border-red-300 focus:border-red-300'
                  : ''
              }
            />
          </div>
        </div>
      </div>

      <DisplayLogicEditor
        formQuestions={formQuestions}
        questionIndex={questionIdx}
        question={question}
        languageCode={languageCode}
        updateQuestion={updateQuestion}
      />
    </div>
  );
}

export default EditNumberQuestion;
