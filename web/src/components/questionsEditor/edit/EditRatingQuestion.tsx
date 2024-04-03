import { AnswerType, BaseAnswer, BaseQuestion, RatingAnswer, RatingQuestion, RatingScaleType, SelectedOptionSchema } from '@/common/types'
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { Button } from '../../ui/button';
import { zodResolver } from '@hookform/resolvers/zod';
import { RatingGroup } from '../../ui/ratings';
import { z } from 'zod';
import { MoveDirection } from '../QuestionsEdit';
import QuestionHeader from './QuestionHeader';
import { useMemo } from 'react';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

export interface EditRatingQuestionProps {
    languageCode: string;
    questionIdx: number;
    activeQuestionId: string | undefined;
    isLastQuestion: boolean;
    isInValid: boolean;
    question: RatingQuestion;
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
    deleteQuestion }: EditRatingQuestionProps) {

    const { t } = useTranslation();
    const options = useMemo(() => [
        { label: t('questionEditor.question.ratingScale.oneTo3'), value: RatingScaleType.OneTo3 },
        { label: t('questionEditor.question.ratingScale.oneTo4'), value: RatingScaleType.OneTo4 },
        { label: t('questionEditor.question.ratingScale.oneTo5'), value: RatingScaleType.OneTo5 },
        { label: t('questionEditor.question.ratingScale.oneTo6'), value: RatingScaleType.OneTo6 },
        { label: t('questionEditor.question.ratingScale.oneTo7'), value: RatingScaleType.OneTo7 },
        { label: t('questionEditor.question.ratingScale.oneTo8'), value: RatingScaleType.OneTo8 },
        { label: t('questionEditor.question.ratingScale.oneTo9'), value: RatingScaleType.OneTo9 },
        { label: t('questionEditor.question.ratingScale.oneTo10'), value: RatingScaleType.OneTo10 },
    ], []);


    function updateRatingScale(scale: RatingScaleType) {
        const updatedRatingQuestion: RatingQuestion = { ...question, scale: scale };
        updateQuestion(questionIdx, updatedRatingQuestion);
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
                <Label htmlFor="scale">{t('questionEditor.ratingQuestion.scale')}</Label>
                        <Select
                            name='scale'
                            value={question.scale}
                            onValueChange={(value: RatingScaleType) => {
                                updateRatingScale(value)
                            }}>
                            <SelectTrigger className='h-8 w-[200px]'>
                                <SelectValue placeholder={question.scale} />
                            </SelectTrigger>
                            <SelectContent side='top'>
                                {options.map((option) => (
                                    <SelectItem key={option.value} value={option.value}>
                                        {option.label}
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
            </div>
        </form>
    )
}

export default EditRatingQuestion
