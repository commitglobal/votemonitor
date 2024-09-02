import { BaseQuestion, FormType, LanguagesTranslationStatus, TranslatedString } from "@/common/types";

export enum FormStatus {
    Drafted = 'Drafted',
    Published = 'Published',
    Obsolete = 'Obsolete',
}

export interface FormBase {
    id: string;
    formType: FormType;
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    description?: TranslatedString;
    status: FormStatus;
    languages: string[];
    createdOn: string;
    lastModifiedOn: string | null;
    numberOfQuestions: number;
    languagesTranslationStatus: LanguagesTranslationStatus;
}

export interface FormFull extends FormBase {
    questions: BaseQuestion[]
}

export interface UpdateFormRequest {
    id: string;
    formType: FormType;
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    description?: TranslatedString;
    languages: string[];
    questions: BaseQuestion[]
}


export interface NewFormRequest {
    code: string;
    defaultLanguage: string;
    name: TranslatedString;
    description: TranslatedString;
    formType: FormType;
    languages: string[];
}
