import { BaseQuestion, DateQuestion, MultiSelectQuestion, NumberQuestion, QuestionType, RatingQuestion, SingleSelectQuestion, TextQuestion } from "@/common/types";


function validateTextQuestion(question: TextQuestion, languageCode: string): boolean { return true; }

function validateNumberQuestion(question: NumberQuestion, languageCode: string): boolean { return true; }

function validateDateQuestion(question: DateQuestion, languageCode: string): boolean { return true; }

function validateSingleSelectQuestion(question: SingleSelectQuestion, languageCode: string): boolean { return true; }

function validateMultiSelectQuestion(question: MultiSelectQuestion, languageCode: string): boolean { return true; }

function validateRatingQuestion(question: RatingQuestion, languageCode: string): boolean { return true; }


const validateQuestion = (question: BaseQuestion, languageCode: string) => {
    switch (question.$questionType) {
        case QuestionType.TextQuestionType: return validateTextQuestion(question as TextQuestion, languageCode);
        case QuestionType.NumberQuestionType: return validateNumberQuestion(question as NumberQuestion, languageCode);
        case QuestionType.DateQuestionType: return validateDateQuestion(question as DateQuestion, languageCode);
        case QuestionType.SingleSelectQuestionType: return validateSingleSelectQuestion(question as SingleSelectQuestion, languageCode);
        case QuestionType.MultiSelectQuestionType: return validateMultiSelectQuestion(question as MultiSelectQuestion, languageCode);
        case QuestionType.RatingQuestionType: return validateRatingQuestion(question as RatingQuestion, languageCode);
    }

    return false;
};

export { validateQuestion };