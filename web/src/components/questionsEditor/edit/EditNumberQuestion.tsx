import { AnswerType, BaseAnswer, BaseQuestion, NumberAnswer, NumberAnswerSchema, NumberQuestion } from '@/common/types'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../../ui/input';
import { Button } from '../../ui/button';
import { useTranslation } from 'react-i18next';
import { MoveDirection } from '../QuestionsEdit';
import QuestionHeader from './QuestionHeader';
import { Label } from '@/components/ui/label';

export interface EditNumberQuestionProps {
  languageCode: string;
  questionIdx: number;
  activeQuestionId: string | undefined;
  isLastQuestion: boolean;
  isInValid: boolean;
  question: NumberQuestion;
  setActiveQuestionId: (questionId: string) => void;
  moveQuestion: (questionIndex: number, direction: MoveDirection) => void;
  updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
  duplicateQuestion: (questionIndex: number) => void;
  deleteQuestion: (questionIndex: number) => void;
}

function EditNumberQuestion({
  languageCode,
  questionIdx,
  activeQuestionId,
  isLastQuestion,
  isInValid,
  question,
  setActiveQuestionId,
  moveQuestion,
  updateQuestion,
  duplicateQuestion,
  deleteQuestion }: EditNumberQuestionProps) {
  const { t } = useTranslation();

  function updateInputPlaceholder(inputPlaceholder: string) {
    const updatedInputPlaceholder = question.inputPlaceholder ? {
      ...question.inputPlaceholder,
      [languageCode]: inputPlaceholder
    } : {
      [languageCode]: inputPlaceholder
    };

    const updatedNumberQuestion: NumberQuestion = { ...question, inputPlaceholder: updatedInputPlaceholder };
    updateQuestion(questionIdx, updatedNumberQuestion);
  }

  return (
    <form>
      <QuestionHeader
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
    </form>


  )
}

export default EditNumberQuestion
