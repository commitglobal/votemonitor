import { AnswerType, BaseAnswer, BaseQuestion, RatingAnswer, RatingQuestion, RatingScaleType } from '@/common/types'
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { Button } from '../../ui/button';
import { zodResolver } from '@hookform/resolvers/zod';
import { RatingGroup } from '../../ui/ratings';
import { z } from 'zod';
import { MoveDirection } from '../QuestionsEdit';
import QuestionHeader from './QuestionHeader';

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

export default EditRatingQuestion
