import { BaseAnswer, SingleSelectAnswer, SingleSelectQuestion, AnswerType, SingleSelectAnswerSchema } from '@/common/types'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../../ui/input';
import { Button } from '../../ui/button';
import { useTranslation } from 'react-i18next';
import { RadioGroup, RadioGroupItem } from '../../ui/radio-group';
import { useMemo, useState } from 'react';

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
  deleteQuestion }: PreviewSingleSelectQuestionProps) {
    const { t } = useTranslation();


    return (<div>Hello EditSingleSelectQuestion</div>)
  }

export default EditSingleSelectQuestion
