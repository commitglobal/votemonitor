import { AnswerType, BaseAnswer, NumberAnswer, TextAnswerSchema, NumberQuestion, TextAnswer, TextQuestion, BaseQuestion } from '@/common/types'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../../ui/input';
import { Button } from '../../ui/button';
import { useTranslation } from 'react-i18next';
import { MoveDirection } from '../QuestionsEdit';

export interface NewQuestionProps {
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

function NewQuestion({ 
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
    deleteQuestion}: NewQuestionProps) {
    const { t } = useTranslation();

    return (<div>I am new question</div>)
}

export default NewQuestion
