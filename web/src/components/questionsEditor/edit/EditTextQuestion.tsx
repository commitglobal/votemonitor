import { BaseQuestion, TextQuestion, newTranslatedString } from '@/common/types';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useTranslation } from 'react-i18next';
import QuestionHeader from './QuestionHeader';
import DisplayLogicEditor from './DisplayLogicEditor';

export interface EditTextQuestionProps {
    formQuestions: BaseQuestion[];
    languageCode: string;
    availableLanguages: string[];
    questionIdx: number;
    isInValid: boolean;
    question: TextQuestion;
    updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
}

function EditTextQuestion({
    formQuestions,
    availableLanguages,
    languageCode,
    questionIdx,
    isInValid,
    question,
    updateQuestion }: EditTextQuestionProps) {
    const { t } = useTranslation();

    function updateInputPlaceholder(inputPlaceholder: string) {
        const updatedInputPlaceholder = question.inputPlaceholder ? {
            ...question.inputPlaceholder,
            [languageCode]: inputPlaceholder
        } : newTranslatedString(availableLanguages, languageCode)

        const updatedTextQuestion: TextQuestion = { ...question, inputPlaceholder: updatedInputPlaceholder };
        updateQuestion(questionIdx, updatedTextQuestion);
    }

    return (
        <div>
            <QuestionHeader
                availableLanguages={availableLanguages}
                languageCode={languageCode}
                isInValid={isInValid}
                question={question}
                questionIdx={questionIdx}
                updateQuestion={updateQuestion}
            />

            <div className="mt-3">
                <Label htmlFor="inputPlaceholder">{t('questionEditor.question.inputPlaceholder')}</Label>
                <div className="mt-2 flex flex-col gap-6">
                    <div className="flex items-center space-x-2">
                        <Input
                            id="inputPlaceholder"
                            name="inputPlaceholder"
                            value={question.inputPlaceholder ? question.inputPlaceholder[languageCode] : ""}
                            onChange={(e) => updateInputPlaceholder(e.target.value)}
                            className={isInValid && !!question.inputPlaceholder && question.inputPlaceholder[languageCode]!.trim() === "" ? "border-red-300 focus:border-red-300" : ""}
                        />
                    </div>
                </div>
            </div>

            <DisplayLogicEditor
                formQuestions={formQuestions}
                questionIndex={questionIdx}
                question={question}
                languageCode={languageCode}
                updateQuestion={updateQuestion} />
        </div>
    )
}
export default EditTextQuestion
