import { BaseQuestion, RatingQuestion, RatingScaleType } from '@/common/types';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import QuestionHeader from './QuestionHeader';

export interface EditRatingQuestionProps {
    availableLanguages: string[];
    languageCode: string;
    questionIdx: number;
    isInValid: boolean;
    question: RatingQuestion;
    updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
}


function EditRatingQuestion({
    availableLanguages,
    languageCode,
    questionIdx,
    isInValid,
    question,
    updateQuestion,
}: EditRatingQuestionProps) {

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
                availableLanguages={availableLanguages}
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
