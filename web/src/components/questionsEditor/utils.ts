import { EditQuestionType } from '@/common/form-requests';
import { QuestionType } from '@/common/types';
import { isNotNilOrWhitespace } from '@/lib/utils';
import {
  Bars3BottomLeftIcon,
  CalculatorIcon,
  CalendarIcon,
  CheckCircleIcon,
  ListBulletIcon,
  StarIcon
} from '@heroicons/react/24/solid';


export function isQuestionTranslated(question: EditQuestionType, languageCode: string, defaultLanguageCode: string): boolean {
  const questionTextValid = isNotNilOrWhitespace(question.text[languageCode]);

  const helptextIsValid = isNotNilOrWhitespace(question.helptext?.[defaultLanguageCode]) ? isNotNilOrWhitespace(question.helptext[languageCode]) : true;

  const isQuestionValid = questionTextValid && helptextIsValid;

  if (question.$questionType === QuestionType.TextQuestionType || question.$questionType === QuestionType.NumberQuestionType) {
    return (isQuestionValid && (isNotNilOrWhitespace(question.inputPlaceholder?.[defaultLanguageCode]) ? isNotNilOrWhitespace(question.inputPlaceholder[languageCode]) : true));
  }

  if (question.$questionType === QuestionType.RatingQuestionType) {
    return (isQuestionValid
      && (isNotNilOrWhitespace(question.lowerLabel?.[defaultLanguageCode]) ? isNotNilOrWhitespace(question.lowerLabel[languageCode]) : true)
      && (isNotNilOrWhitespace(question.upperLabel?.[defaultLanguageCode]) ? isNotNilOrWhitespace(question.upperLabel[languageCode]) : true)
    );
  }

  if (question.$questionType === QuestionType.SingleSelectQuestionType || question.$questionType === QuestionType.MultiSelectQuestionType) {
    return isQuestionValid && question.options.every((option) => isNotNilOrWhitespace(option.text[languageCode]));
  }

  return isQuestionValid;
}

export const questionsIconMapping = {
  [QuestionType.TextQuestionType]: Bars3BottomLeftIcon,
  [QuestionType.NumberQuestionType]: CalculatorIcon,
  [QuestionType.DateQuestionType]: CalendarIcon,
  [QuestionType.RatingQuestionType]: StarIcon,
  [QuestionType.SingleSelectQuestionType]: CheckCircleIcon,
  [QuestionType.MultiSelectQuestionType]: ListBulletIcon,
};