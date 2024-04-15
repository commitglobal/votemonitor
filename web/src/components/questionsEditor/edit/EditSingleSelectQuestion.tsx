import { BaseQuestion } from '@/common/types';

import { useTranslation } from 'react-i18next';

import { MoveDirection } from '../QuestionsEdit';

export interface PreviewSingleSelectQuestionProps {
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

function EditSingleSelectQuestion({
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
  deleteQuestion,
}: PreviewSingleSelectQuestionProps) {
  const { t } = useTranslation();

  return <div>Hello EditSingleSelectQuestion</div>;
}

export default EditSingleSelectQuestion;
