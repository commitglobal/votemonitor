import { BaseQuestion, DateQuestion } from '@/common/types'
import { useTranslation } from 'react-i18next';
import { MoveDirection } from '../QuestionsEdit';
import { Button } from '@/components/ui/button';
import { Form } from '@/components/ui/form';
import QuestionHeader from './QuestionHeader';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';

export interface EditDateQuestionProps {
  languageCode: string;
  questionIdx: number;
  activeQuestionId: string | undefined;
  isLastQuestion: boolean;
  isInValid: boolean;
  question: DateQuestion;
  setActiveQuestionId: (questionId: string) => void;
  moveQuestion: (questionIndex: number, direction: MoveDirection) => void;
  updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
  duplicateQuestion: (questionIndex: number) => void;
  deleteQuestion: (questionIndex: number) => void;
}

function EditDateQuestion({
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
  deleteQuestion }: EditDateQuestionProps) {
  const { t } = useTranslation();


  return (
    <form>
      <QuestionHeader
        languageCode={languageCode}
        isInValid={isInValid}
        question={question}
        questionIdx={questionIdx}
        updateQuestion={updateQuestion}
      />
    </form>
  )
}


export default EditDateQuestion
