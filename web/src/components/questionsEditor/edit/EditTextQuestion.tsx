import { BaseQuestion, TextQuestion } from '@/common/types'
import { useTranslation } from 'react-i18next';
import { MoveDirection } from '../QuestionsEdit';
import QuestionHeader from './QuestionHeader';

export interface EditTextQuestionProps {
    languageCode: string;
    questionIdx: number;
    activeQuestionId: string | undefined;
    isLastQuestion: boolean;
    isInValid: boolean;
    question: TextQuestion;
    setActiveQuestionId: (questionId: string) => void;
    moveQuestion: (questionIndex: number, direction: MoveDirection) => void;
    updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
    duplicateQuestion: (questionIndex: number) => void;
    deleteQuestion: (questionIndex: number) => void;
}

function EditTextQuestion({
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
    deleteQuestion }: EditTextQuestionProps) {
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
export default EditTextQuestion
