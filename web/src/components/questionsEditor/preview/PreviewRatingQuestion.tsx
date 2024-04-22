import { RatingAnswer, RatingQuestion, RatingScaleType } from '@/common/types'
import { RatingGroup } from '../../ui/ratings';
import { Description, Field, Label } from '@/components/ui/fieldset';

export interface PreviewRatingQuestionProps {
    languageCode: string;
    question: RatingQuestion;
    answer: RatingAnswer;
    setAnswer: (answer: RatingAnswer) => void;
}

function scaleToNumber(scale: RatingScaleType): number {
    switch (scale) {
        case RatingScaleType.OneTo3: return 3;
        case RatingScaleType.OneTo4: return 4;
        case RatingScaleType.OneTo5: return 5;
        case RatingScaleType.OneTo6: return 6;
        case RatingScaleType.OneTo7: return 7;
        case RatingScaleType.OneTo8: return 8;
        case RatingScaleType.OneTo9: return 9;
        case RatingScaleType.OneTo10: return 10;
        default:
            return 5;
    }
}

function PreviewRatingQuestion({ languageCode,
    question,
    answer,
    setAnswer }: PreviewRatingQuestionProps) {

    return (
        <Field>
            <Label>{question.code} - {question.text[languageCode]}</Label>
            {!!question.helptext && <Description>{question.helptext[languageCode]}</Description>}
            <RatingGroup
                className='my-2'
                scale={scaleToNumber(question.scale)}
                name='value' />
        </Field>
    )
}


export default PreviewRatingQuestion
