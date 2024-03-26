import { BaseAnswer, RatingAnswer, RatingQuestion } from '@/common/types'

export interface PreviewRatingQuestionProps {
    languageCode: string;
    question: RatingQuestion;
    answer: RatingAnswer;
    isFirstQuestion: boolean;
    isLastQuestion: boolean;
    onSubmitAnswer: (answer: BaseAnswer) => void;
    onBackButtonClicked: () => void;
}

function PreviewRatingQuestion({ onSubmitAnswer, onBackButtonClicked }: PreviewRatingQuestionProps) {
    return (
        <div>PreviewRatingQuestion</div>
    )
}


export default PreviewRatingQuestion
