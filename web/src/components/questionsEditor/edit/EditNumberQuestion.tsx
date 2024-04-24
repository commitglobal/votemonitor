import { BaseQuestion, NumberQuestion, newTranslatedString } from '@/common/types';
import { Label } from '@/components/ui/label';
import { useTranslation } from 'react-i18next';
import { Input } from '../../ui/input';
import QuestionHeader from './QuestionHeader';

export interface EditNumberQuestionProps {
  languageCode: string;
  availableLanguages: string[];
  questionIdx: number;
  isInValid: boolean;
  question: NumberQuestion;
  updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
}

function EditNumberQuestion({
  availableLanguages,
  languageCode,
  questionIdx,
  isInValid,
  question,
  updateQuestion }: EditNumberQuestionProps) {
  const { t } = useTranslation();

  function updateInputPlaceholder(inputPlaceholder: string) {
    const updatedInputPlaceholder = question.inputPlaceholder ? {
      ...question.inputPlaceholder,
      [languageCode]: inputPlaceholder
    } : newTranslatedString(availableLanguages, languageCode)

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

      <div className="mt-3">
        <Label htmlFor="inputPlaceholder">{t('questionEditor.question.inputPlaceholder')}</Label>
        <div className="mt-2 flex flex-col gap-6">
          <div className="flex items-center space-x-2">
            <Input
              id="inputPlaceholder"
              name="inputPlaceholder"
              value={question.inputPlaceholder ? question.inputPlaceholder[languageCode] : ""}
              onChange={(e) => updateInputPlaceholder(e.target.value)}
              className={isInValid && !!question.inputPlaceholder && question.inputPlaceholder[languageCode]!.trim() === "" ? "border-red-300 focus:border-red-300" : ""}
            />
          </div>
        </div>
      </div>
    </div>
  )
}

export default EditNumberQuestion
