import { TextAnswer, TextQuestion } from '@/common/types'
import { Description, Field, Label } from '@/components/ui/fieldset';
import { Textarea } from '../../ui/textarea';

export interface PreviewTextQuestionProps {
    languageCode: string;
    question: TextQuestion;
    answer?: TextAnswer;
    setAnswer: (answer: TextAnswer) => void;
}

function PreviewTextQuestion({
    languageCode,
    question,
    answer,
    setAnswer }: PreviewTextQuestionProps) {
    return (
        <Field>
            <Label>{question.code} - {question.text[languageCode]}</Label>
            {!!question.helptext && <Description>{question.helptext[languageCode]}</Description>}
            <Textarea placeholder={question.inputPlaceholder ? question.inputPlaceholder[languageCode] : ''} />
        </Field>)
}

export default PreviewTextQuestion
