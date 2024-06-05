import { BaseQuestion, QuestionType, newTranslatedString } from '@/common/types';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useParams } from '@tanstack/react-router';
import { RefObject } from 'react';
import { useTranslation } from 'react-i18next';

const questionPlaceholders: Record<
  QuestionType,
  { code: string; text: (availableLanguages: string[], languageCode: string) => string | undefined }
> = {
  textQuestion: {
    code: 'TQ',
    text: (availableLanguages, languageCode) =>
      newTranslatedString(availableLanguages, languageCode, 'Text question text')[languageCode],
  },
  dateQuestion: {
    code: 'DQ',
    text: (availableLanguages, languageCode) =>
      newTranslatedString(availableLanguages, languageCode, 'Date question text')[languageCode],
  },
  numberQuestion: {
    code: 'NQ',
    text: (availableLanguages, languageCode) =>
      newTranslatedString(availableLanguages, languageCode, 'Number question text')[languageCode],
  },
  ratingQuestion: {
    code: 'RQ',
    text: (availableLanguages, languageCode) =>
      newTranslatedString(availableLanguages, languageCode, 'Rating question text')[languageCode],
  },
  singleSelectQuestion: {
    code: 'SQ',
    text: (availableLanguages, languageCode) =>
      newTranslatedString(availableLanguages, languageCode, 'Single choice question text')[languageCode],
  },
  multiSelectQuestion: {
    code: 'MQ',
    text: (availableLanguages, languageCode) =>
      newTranslatedString(availableLanguages, languageCode, 'Multi choice question text')[languageCode],
  },
};

interface QuestionHeaderProps {
  availableLanguages: string[];
  languageCode: string;
  question: BaseQuestion;
  questionIdx: number;
  isInValid: boolean;
  ref?: RefObject<HTMLInputElement>;
  updateQuestion: (questionIdx: number, updatedAttributes: any) => void;
}

function QuestionHeader({
  availableLanguages,
  languageCode,
  question,
  questionIdx,
  isInValid,
  ref,
  updateQuestion,
}: QuestionHeaderProps) {
  const { t } = useTranslation();

  const params: any = useParams({
    strict: false,
  });

  function updateText(text: string) {
    const updatedText = {
      ...question.text,
      [params.languageCode ? params.languageCode : languageCode]: text,
    };

    updateQuestion(questionIdx, { ...question, text: updatedText });
  }

  function updateHelptext(helptext: string) {
    const updatedHelptext = question.helptext
      ? {
          ...question.helptext,
          [params.languageCode ? params.languageCode : languageCode]: helptext,
        }
      : newTranslatedString(availableLanguages, languageCode, helptext);

    updateQuestion(questionIdx, { ...question, helptext: updatedHelptext });
  }

  return (
    <>
      <div className='mt-3'>
        <Label htmlFor='code'>{t('questionEditor.question.code')}</Label>
        <div className='mt-2 flex flex-col gap-6'>
          <div className='flex items-center space-x-2'>
            <Input
              autoFocus
              ref={ref}
              id='code'
              name='code'
              value={question.code}
              onChange={(e) => updateQuestion(questionIdx, { ...question, code: e.target.value })}
              className={isInValid && question.code.trim() === '' ? 'border-red-300 focus:border-red-300' : ''}
              disabled={!!params['languageCode']}
              placeholder={questionPlaceholders[question.$questionType].code}
            />
          </div>
        </div>
      </div>
      <div className='mt-3'>
        <Label htmlFor='text'>{t('questionEditor.question.text')}</Label>
        <div className='mt-2 flex flex-col gap-6'>
          <div className='flex items-center space-x-2'>
            <Input
              id='text'
              name='text'
              value={params['languageCode'] ? question.text[params['languageCode']] : question.text[languageCode]}
              placeholder={questionPlaceholders[question.$questionType].text(
                availableLanguages,
                params['languageCode'] ?? languageCode
              )}
              onChange={(e) => updateText(e.target.value)}
              className={
                isInValid && question.text[languageCode]!.trim() === '' ? 'border-red-300 focus:border-red-300' : ''
              }
            />
          </div>
        </div>
      </div>
      <div className='mt-3'>
        <Label htmlFor='helptext'>{t('questionEditor.question.helptext')}</Label>
        <div className='mt-2 flex flex-col gap-6'>
          <div className='flex items-center space-x-2'>
            <Input
              id='helptext'
              name='helptext'
              value={
                params['languageCode'] ? question.helptext?.[params['languageCode']] : question.helptext?.[languageCode]
              }
              placeholder={params['languageCode'] ? question.helptext?.[languageCode] : ''}
              onChange={(e) => updateHelptext(e.target.value)}
              className={
                isInValid && !!question.helptext && question.helptext[languageCode]!.trim() === ''
                  ? 'border-red-300 focus:border-red-300'
                  : ''
              }
            />
          </div>
        </div>
      </div>
    </>
  );
}

export default QuestionHeader;
