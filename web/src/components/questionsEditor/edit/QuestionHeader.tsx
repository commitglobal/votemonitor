import { BaseQuestion, newTranslatedString } from '@/common/types';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { RefObject } from 'react';
import { useTranslation } from 'react-i18next';

interface QuestionHeaderProps {
    availableLanguages: string[];
    languageCode: string;
    question: BaseQuestion;
    questionIdx: number;
    isInValid: boolean;
    ref?: RefObject<HTMLInputElement>;
    updateQuestion: (questionIdx: number, updatedAttributes: any) => void;
}

function QuestionHeader({
    availableLanguages,
    languageCode,
    question,
    questionIdx,
    isInValid,
    ref,
    updateQuestion
}: QuestionHeaderProps) {

    const { t } = useTranslation();

    function updateText(text: string) {
        const updatedText = {
            ...question.text,
            [languageCode]: text
        }

        updateQuestion(questionIdx, { ...question, text: updatedText });
    }

    function updateHelptext(helptext: string) {
        const updatedHelptext = question.helptext ? {
            ...question.helptext,
            [languageCode]: helptext
        } : newTranslatedString(availableLanguages, languageCode, helptext);

        updateQuestion(questionIdx, { ...question, helptext: updatedHelptext });
    }

    return (
        <>
            <div className="mt-3">
                <Label htmlFor="code">{t('questionEditor.question.code')}</Label>
                <div className="mt-2 flex flex-col gap-6">
                    <div className="flex items-center space-x-2">
                        <Input
                            autoFocus
                            ref={ref}
                            id="code"
                            name="code"
                            value={question.code}
                            onChange={(e) => updateQuestion(questionIdx, { ...question, code: e.target.value })}
                            className={isInValid && question.code.trim() === "" ? "border-red-300 focus:border-red-300" : ""}
                        />
                    </div>
                </div>
            </div>
            <div className="mt-3">
                <Label htmlFor="text">{t('questionEditor.question.text')}</Label>
                <div className="mt-2 flex flex-col gap-6">
                    <div className="flex items-center space-x-2">
                        <Input
                            id="text"
                            name="text"
                            value={question.text[languageCode]}
                            onChange={(e) => updateText(e.target.value)}
                            className={isInValid && question.text[languageCode]!.trim() === "" ? "border-red-300 focus:border-red-300" : ""}
                        />
                    </div>
                </div>
            </div>
            <div className="mt-3">
                <Label htmlFor="helptext">{t('questionEditor.question.helptext')}</Label>
                <div className="mt-2 flex flex-col gap-6">
                    <div className="flex items-center space-x-2">
                        <Input
                            id="helptext"
                            name="helptext"
                            value={question.helptext ? question.helptext[languageCode] : ""}
                            onChange={(e) => updateHelptext(e.target.value)}
                            className={isInValid && !!question.helptext && question.helptext[languageCode]!.trim() === "" ? "border-red-300 focus:border-red-300" : ""}
                        />
                    </div>
                </div>
            </div>
        </>
    )
}

export default QuestionHeader