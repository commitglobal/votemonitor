import { AnswerType, BaseAnswer, NumberAnswer, TextAnswerSchema, NumberQuestion, TextAnswer, TextQuestion } from '@/common/types'
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../../ui/input';
import { useTranslation } from 'react-i18next';
import { Description, Field, Label } from '@/components/ui/fieldset';
import { Textarea } from '../../ui/textarea';

export interface PreviewTextQuestionProps {
    languageCode: string;
    question: TextQuestion;
    answer: TextAnswer;
    isFirstQuestion: boolean;
    isLastQuestion: boolean;
    onSubmitAnswer: (answer: BaseAnswer) => void;
    onBackButtonClicked: () => void;
}

function PreviewTextQuestion({
    languageCode,
    question,
    answer,
    isFirstQuestion,
    isLastQuestion,
    onSubmitAnswer,
    onBackButtonClicked }: PreviewTextQuestionProps) {
    const { t } = useTranslation();

    const form = useForm<TextAnswer>({
        resolver: zodResolver(TextAnswerSchema),
        defaultValues: answer ?? {
            questionId: question.id,
            $answerType: AnswerType.TextAnswerType,
        }
    });

    return (
        <Field>
            <Label>{question.code} - {question.text[languageCode]}</Label>
            {!!question.helptext && <Description>{question.helptext[languageCode]}</Description>}
            <Textarea placeholder={question.inputPlaceholder ? question.inputPlaceholder[languageCode] : ''} />
        </Field>)
}

export default PreviewTextQuestion
