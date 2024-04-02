import { AnswerType, BaseAnswer, BaseQuestion, RatingAnswer, RatingQuestion, RatingScaleType } from '@/common/types'
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { Button } from '../../ui/button';
import { zodResolver } from '@hookform/resolvers/zod';
import { RatingGroup } from '../../ui/ratings';
import { z } from 'zod';
import { MoveDirection } from '../QuestionsEdit';

export interface EditRatingQuestionProps {
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


function EditRatingQuestion({ 
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
  deleteQuestion}: EditRatingQuestionProps) {

    const { t } = useTranslation();


    return (<div>Hello EditRatingQuestion</div>)
}


export default EditRatingQuestion
