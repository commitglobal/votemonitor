import { AnswerType, BaseAnswer, BaseQuestion, NumberAnswer, NumberAnswerSchema, NumberQuestion } from '@/common/types'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../../ui/input';
import { Button } from '../../ui/button';
import { useTranslation } from 'react-i18next';
import { MoveDirection } from '../QuestionsEdit';

export interface EditNumberQuestionProps {
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
  deleteQuestion}: EditNumberQuestionProps) {
  const { t } = useTranslation();


  return (<div>Hello EditDateQuestion</div>)
}
export default EditNumberQuestion
