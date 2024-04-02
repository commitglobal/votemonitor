import { BaseQuestion } from '@/common/types'
import { useTranslation } from 'react-i18next';
import { MoveDirection } from '../QuestionsEdit';

export interface EditDateQuestionProps {
  languageCode: string;
  questionIdx: number;
  activeQuestionId: string | undefined;
  isLastQuestion: boolean;
  isInValid: boolean;
  question: BaseQuestion | undefined;
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


  return (<div>Hello EditDateQuestion</div>)
}


export default EditDateQuestion
