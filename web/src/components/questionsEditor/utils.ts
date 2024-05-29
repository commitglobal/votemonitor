import { isMultiSelectQuestion, isNumberQuestion, isSingleSelectQuestion, isTextQuestion } from '@/common/guards';
import type { BaseQuestion } from '@/common/types';

export function isQuestionTranslated(question: BaseQuestion, languageCode: string, paramLanguageCode: string): boolean {
  const textRequired = question.text[paramLanguageCode] !== '';
  const helpTextRule = question.helptext?.[languageCode] ? question.helptext?.[paramLanguageCode] !== '' : true;

  const genericProperties = textRequired && helpTextRule;

  if (isTextQuestion(question) || isNumberQuestion(question)) {
    return (
      genericProperties &&
      (question.inputPlaceholder?.[languageCode] ? question.inputPlaceholder?.[paramLanguageCode] !== '' : true)
    );
  }

  if (isSingleSelectQuestion(question) || isMultiSelectQuestion(question)) {
    return genericProperties && question.options.every((option) => option.text[paramLanguageCode] !== '');
  }

  return genericProperties;
}
