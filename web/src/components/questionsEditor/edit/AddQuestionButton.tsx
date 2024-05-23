import { useState } from 'react';
import {
  PlusIcon,
  Bars3BottomLeftIcon,
  CalculatorIcon,
  CalendarIcon,
  StarIcon,
  CheckCircleIcon,
  ListBulletIcon,
} from '@heroicons/react/24/solid';
import { Collapsible, CollapsibleTrigger, CollapsibleContent } from '@/components/ui/collapsible';
import { cn } from '@/lib/utils';
import { v4 as uuidv4 } from 'uuid';
import {
  BaseQuestion,
  DateQuestion,
  MultiSelectQuestion,
  NumberQuestion,
  QuestionType,
  RatingQuestion,
  RatingScaleType,
  SingleSelectQuestion,
  TextQuestion,
  newTranslatedString,
} from '@/common/types';
import i18n from '@/i18n';

export type QuestionTypeConfig = {
  type: QuestionType;
  label: string;
  icon: any;
  create: (availableLanguages: string[], languageCode: string) => BaseQuestion;
};

const questionTypes: QuestionTypeConfig[] = [
  {
    type: QuestionType.TextQuestionType,
    icon: Bars3BottomLeftIcon,
    label: i18n.t('questionEditor.questionType.textQuestion'),
    create: (availableLanguages: string[], languageCode: string) => {
      const newTextQuestion: TextQuestion = {
        id: uuidv4(),
        $questionType: QuestionType.TextQuestionType,
        code: 'TQ',
        inputPlaceholder: newTranslatedString(availableLanguages, languageCode, ''),
        helptext: newTranslatedString(availableLanguages, languageCode, ''),
        text: newTranslatedString(availableLanguages, languageCode, 'Text question text'),
      };

      return newTextQuestion;
    },
  },
  {
    type: QuestionType.NumberQuestionType,
    icon: CalculatorIcon,
    label: i18n.t('questionEditor.questionType.numberQuestion'),
    create: (availableLanguages: string[], languageCode: string) => {
      const newNumberQuestion: NumberQuestion = {
        id: uuidv4(),
        $questionType: QuestionType.NumberQuestionType,
        code: 'NQ',
        helptext: newTranslatedString(availableLanguages, languageCode, ''),
        text: newTranslatedString(availableLanguages, languageCode, 'Number question text'),
      };

      return newNumberQuestion;
    },
  },
  {
    type: QuestionType.DateQuestionType,
    icon: CalendarIcon,
    label: i18n.t('questionEditor.questionType.dateQuestion'),
    create: (availableLanguages: string[], languageCode: string) => {
      const newDateQuestion: DateQuestion = {
        id: uuidv4(),
        $questionType: QuestionType.DateQuestionType,
        code: 'DQ',
        helptext: newTranslatedString(availableLanguages, languageCode, ''),
        text: newTranslatedString(availableLanguages, languageCode, 'Date question text'),
      };

      return newDateQuestion;
    },
  },
  {
    type: QuestionType.RatingQuestionType,
    icon: StarIcon,
    label: i18n.t('questionEditor.questionType.ratingQuestion'),
    create: (availableLanguages: string[], languageCode: string) => {
      const newRatingQuestion: RatingQuestion = {
        id: uuidv4(),
        $questionType: QuestionType.RatingQuestionType,
        code: 'RQ',
        helptext: newTranslatedString(availableLanguages, languageCode, ''),
        text: newTranslatedString(availableLanguages, languageCode, 'Rating question text'),
        scale: RatingScaleType.OneTo5,
      };

      return newRatingQuestion;
    },
  },
  {
    type: QuestionType.SingleSelectQuestionType,
    icon: CheckCircleIcon,
    label: i18n.t('questionEditor.questionType.singleSelectQuestion'),
    create: (availableLanguages: string[], languageCode: string) => {
      const newSingleSelectQuestion: SingleSelectQuestion = {
        id: uuidv4(),
        $questionType: QuestionType.SingleSelectQuestionType,
        code: 'SC',
        helptext: newTranslatedString(availableLanguages, languageCode, ''),
        text: newTranslatedString(availableLanguages, languageCode, 'Single choice question text'),
        options: [
          {
            id: uuidv4(),
            text: newTranslatedString(availableLanguages, languageCode, 'Option 1'),
            isFlagged: false,
            isFreeText: false,
          },
          {
            id: uuidv4(),
            text: newTranslatedString(availableLanguages, languageCode, 'Option 2'),
            isFlagged: false,
            isFreeText: false,
          },
        ],
      };

      return newSingleSelectQuestion;
    },
  },
  {
    type: QuestionType.MultiSelectQuestionType,
    icon: ListBulletIcon,
    label: i18n.t('questionEditor.questionType.multiSelectQuestion'),
    create: (availableLanguages: string[], languageCode: string) => {
      const newMultiSelectQuestion: MultiSelectQuestion = {
        id: uuidv4(),
        $questionType: QuestionType.MultiSelectQuestionType,
        code: 'MC',
        helptext: newTranslatedString(availableLanguages, languageCode, ''),
        text: newTranslatedString(availableLanguages, languageCode, 'Multi choice question text'),
        options: [
          {
            id: uuidv4(),
            text: newTranslatedString(availableLanguages, languageCode),
            isFlagged: false,
            isFreeText: false,
          },
        ],
      };

      return newMultiSelectQuestion;
    },
  },
];

interface AddQuestionButtonProps {
  availableLanguages: string[];
  languageCode: string;
  addQuestion: (question: BaseQuestion) => void;
}

export default function AddQuestionButton({ availableLanguages, languageCode, addQuestion }: AddQuestionButtonProps) {
  const [open, setOpen] = useState(false);

  return (
    <Collapsible
      open={open}
      onOpenChange={setOpen}
      className={cn(
        open ? 'scale-100 shadow-lg' : 'scale-97 shadow-md',
        'group w-full space-y-2 rounded-lg border  border-slate-300 bg-white transition-all duration-300 ease-in-out hover:scale-100 hover:cursor-pointer hover:bg-slate-50'
      )}>
      <CollapsibleTrigger asChild className='group h-full w-full'>
        <div className='inline-flex'>
          <div className='bg-primary flex w-10 items-center justify-center rounded-l-lg group-aria-expanded:rounded-bl-none group-aria-expanded:rounded-br'>
            <PlusIcon className='h-6 w-6 text-white' />
          </div>
          <div className='px-4 py-3'>
            <p className='font-semibold'>Add Question</p>
            <p className='mt-1 text-sm text-slate-500'>Add a new question to your form</p>
          </div>
        </div>
      </CollapsibleTrigger>
      <CollapsibleContent className='justify-left flex flex-col '>
        {questionTypes.map((questionType) => (
          <button
            type='button'
            key={questionType.type}
            className='mx-2 inline-flex items-center rounded p-0.5 px-4 py-2 font-medium text-slate-700 last:mb-2 hover:bg-slate-100 hover:text-slate-800'
            onClick={() => {
              addQuestion(questionType.create(availableLanguages, languageCode));
              setOpen(false);
            }}>
            <questionType.icon className='text-primary -ml-0.5 mr-2 h-5 w-5' aria-hidden='true' />
            {questionType.label}
          </button>
        ))}
      </CollapsibleContent>
    </Collapsible>
  );
}
